# جریان کامل API — به ترتیب وضعیت

## وضعیت‌های درخواست (RequestStatus)

| مقدار | توضیح |
|---|---|
| `Pending` | ثبت شده، منتظر مچ |
| `Matched` | مچ شده |
| `Expired` | منقضی شده (بدون فعالیت) |
| `Cancelled` | لغو شده توسط کاربر |

## وضعیت‌های مچ (MatchStatus)

| مقدار | توضیح |
|---|---|
| `PendingConfirmation` | ادمین مچ ایجاد کرده، منتظر تایید دو طرف |
| `Active` | هر دو طرف تایید کردند، تراکنش جاری |
| `Completed` | تراکنش نهایی شده |
| `Cancelled` | رد شده یا لغو شده |

## وضعیت‌های تراکنش (TransactionStatus)

| مقدار | توضیح |
|---|---|
| `WaitingForTomanPayment` | منتظر پرداخت تومان توسط گیرنده ارز |
| `TomanPaymentDeclared` | گیرنده ارز اعلام کرده تومان پرداخت شده |
| `TomanConfirmed` | ادمین تومان را تایید کرده |
| `ForeignReceiptUploaded` | ارسال‌کننده ارز رسید ارسال آپلود کرده |
| `ForeignReceiptConfirmed` | گیرنده ارز دریافت را تایید کرده، منتظر تسویه ادمین |
| `Completed` | ادمین تومان را به گیرنده واریز و مچ را نهایی کرده |
| `Disputed` | اختلاف |

---

## مرحله ۱ — ثبت درخواست

### `POST /api/requests`
**Actor:** کاربر  
**وضعیت بعد:** Request = `Pending`

```json
{
  "type": "Send | Receive",
  "currency": "EUR | USD | GBP | ...",
  "amount": 1000,
  "pricePreference": "Floating | Fixed",
  "fixedPrice": 62000,
  "receiver": {
    "iban": "DE89...",
    "ibanOwnerName": "Ali Mohammadi"
  },
  "tomanPayer": {
    "fullName": "Sara Ahmadi",
    "mobileNumber": "09121234567"
  }
}
```

**نکته:** `receiver` فقط برای `type = Send` لازم است. `tomanPayer` فقط برای `type = Receive`.

---

### `DELETE /api/requests/{id}`
**Actor:** کاربر  
**وضعیت بعد:** Request = `Cancelled`  
درخواستی که هنوز `Pending` است قابل لغو است.

---

### `GET /api/requests`
**Actor:** کاربر — لیست درخواست‌های قابل مشاهده (خود + مچ‌شده‌ها)  
Query: `?type=Send|Receive`

### `GET /api/requests/mine`
**Actor:** کاربر — فقط درخواست‌های خود

### `GET /api/requests/{id}`
**Actor:** کاربر — جزئیات یک درخواست

### `GET /api/requests/admin`
**Actor:** Admin/Agent — لیست همه درخواست‌ها  
Query: `?type=Send|Receive&currency=EUR&amount=1000`

### `GET /api/requests/search`
**Actor:** کاربر — جستجو برای درخواست‌های هم‌خوان  
Query: `?type=Send|Receive&currency=EUR&amount=1000`

---

## مرحله ۲ — مچ کردن

### `POST /api/matches/admin`
**Actor:** Admin/Agent  
**وضعیت بعد:** Match = `PendingConfirmation`، Request‌ها = `Matched`

```json
{
  "senderRequestId": "guid",
  "receiverRequestId": "guid",
  "price": 62000
}
```

---

### `GET /api/matches/admin/pending-confirmation`
**Actor:** Admin/Agent — لیست مچ‌های منتظر تایید کاربران

### `GET /api/matches/pending-confirmation`
**Actor:** کاربر — مچ‌هایی که این کاربر باید تایید یا رد کند

### `GET /api/matches/my`
**Actor:** کاربر — همه مچ‌های این کاربر

---

## مرحله ۳ — تایید یا رد مچ توسط کاربران

### `POST /api/matches/{id}/confirm`
**Actor:** هر یک از دو طرف مچ  
**وضعیت بعد:**  
- اگر فقط یک طرف تایید کرده: Match هنوز `PendingConfirmation`  
- اگر هر دو تایید کردند: Match = `Active`، Transaction ایجاد می‌شود با وضعیت `WaitingForTomanPayment`

**نکته:** اگر `ConfirmationDeadline` گذشته باشد → `400 Bad Request`.  
در صورت انقضا، match بصورت خودکار `Cancelled` می‌شود (در پاسخ به درخواست بعدی بررسی می‌شود).

---

### `POST /api/matches/{id}/reject`
**Actor:** هر یک از دو طرف مچ  
**وضعیت بعد:** Match = `Cancelled`

---

## مرحله ۴ — پرداخت تومان

### `GET /api/matches/{id}/payment-invoice`
**Actor:** هر دو طرف مچ، Admin  
اطلاعات واریز تومان (Base64 encoded JSON).  
شامل: IBAN گیرنده تومان، مبلغ، کد رهگیری، مهلت پرداخت.

---

### `POST /api/transactions/{id}/declare-toman-payment`
**Actor:** گیرنده ارز (پرداخت‌کننده تومان)  
**وضعیت بعد:** Transaction = `TomanPaymentDeclared`

---

## مرحله ۵ — تایید تومان توسط ادمین

### `POST /api/transactions/{id}/confirm-toman`
**Actor:** Admin  
**وضعیت بعد:** Transaction = `TomanConfirmed`

---

## مرحله ۶ — ارسال ارز

### `POST /api/transactions/{id}/foreign-receipt`
**Actor:** ارسال‌کننده ارز  
**وضعیت بعد:** Transaction = `ForeignReceiptUploaded`  
Body: `multipart/form-data` با فایل رسید

---

## مرحله ۷ — تایید دریافت ارز

### `POST /api/transactions/{id}/confirm-foreign`
**Actor:** گیرنده ارز  
**وضعیت بعد:** Transaction = `ForeignReceiptConfirmed`

---

## مرحله ۸ — تسویه نهایی توسط ادمین

### `POST /api/transactions/{id}/admin-settle`
**Actor:** Admin  
**وضعیت بعد:** Transaction = `Completed`، Match = `Completed`  
ادمین تومان را به حساب گیرنده تومان واریز می‌کند و مچ را نهایی می‌کند.

---

## سایر endpoint‌ها

### `GET /api/transactions`
**Actor:** کاربر — لیست تراکنش‌های خود  
Query: `?type=Send|Receive&status=WaitingForTomanPayment|...&page=1&pageSize=20`

### `GET /api/transactions/{id}`
**Actor:** کاربر — جزئیات تراکنش

---

## خلاصه جریان

```
[کاربر] POST /requests
           ↓
[Admin] POST /matches/admin          → Match: PendingConfirmation
           ↓
[هر دو] POST /matches/{id}/confirm   → Match: Active, Transaction ایجاد
           ↓                          (یا reject → Cancelled)
[گیرنده ارز] POST /transactions/{id}/declare-toman-payment
           ↓
[Admin] POST /transactions/{id}/confirm-toman
           ↓
[ارسال‌کننده ارز] GET /matches/{id}/payment-invoice  (اطلاعات انتقال)
[ارسال‌کننده ارز] POST /transactions/{id}/foreign-receipt
           ↓
[گیرنده ارز] POST /transactions/{id}/confirm-foreign
           ↓
[Admin] POST /transactions/{id}/admin-settle  → Completed ✓
```

---

## وضعیت‌های پایانی

| وضعیت | Match | Transaction |
|---|---|---|
| نهایی موفق | `Completed` | `Completed` |
| رد شده | `Cancelled` | — |
| لغو درخواست | — | — (Request: `Cancelled`) |
| منقضی شده | `Cancelled` | — (Request: `Expired`) |
| اختلاف | `Active` | `Disputed` |
