# GET /api/matches/{id}/payment-invoice

اطلاعات فاکتور پرداخت تومان برای یک match تایید‌شده.

## Endpoint

```
GET /api/matches/{id}/payment-invoice
Authorization: Bearer <token>
```

## دسترسی

| نقش | مجاز |
|---|---|
| شرکت‌کننده در match (sender یا receiver) | بله |
| Admin | بله |

## شرط

match باید `Active` باشد — یعنی هر دو طرف تایید کرده باشند.  
اگر match هنوز `PendingConfirmation` باشد → `400 Bad Request`.

---

## پاسخ موفق `200 OK`

```json
{
  "encoded": "<base64 string>"
}
```

`encoded` = JSON زیر که UTF-8 encode شده و سپس Base64 گرفته شده.

### decode در فرانت

```js
const json = atob(response.encoded);         // browser
const obj  = JSON.parse(json);
```

```js
const json = Buffer.from(response.encoded, 'base64').toString(); // Node.js
const obj  = JSON.parse(json);
```

---

### ساختار JSON داخل encoded

```json
{
  "sender": {
    "name": "علی محمدی",
    "phone": "09121234567",
    "fee": 1.00,
    "amount": 101000
  },
  "receiver": {
    "iban": "IR123456789012345678901234",
    "ibanOwnerName": "سارا احمدی",
    "fee": 2.00,
    "amount": 98000
  },
  "amount": 100000,
  "invoiceNumber": "TX-ABC123-XYZ",
  "expireAt": "2026-06-02T10:00:00Z"
}
```

### توضیح فیلدها

#### sender — پرداخت‌کننده تومان (دریافت‌کننده یورو)

| فیلد | نوع | توضیح |
|---|---|---|
| `name` | string | نام کامل — از `tomanPayer.fullName` درخواست Receive |
| `phone` | string | موبایل — از `tomanPayer.mobileNumber` درخواست Receive |
| `fee` | decimal | درصد کمیسیون تیر این کاربر (ReceiverCommissionPercent) |
| `amount` | decimal | مبلغ **پرداختی** به تومان (base + کمیسیون) |

#### receiver — دریافت‌کننده تومان (ارسال‌کننده یورو)

| فیلد | نوع | توضیح |
|---|---|---|
| `iban` | string | شماره IBAN گیرنده تومان |
| `ibanOwnerName` | string | نام صاحب IBAN |
| `fee` | decimal | درصد کمیسیون تیر این کاربر (SenderCommissionPercent) |
| `amount` | decimal | مبلغ **دریافتی** به تومان (base - کمیسیون) |

#### سایر فیلدها

| فیلد | نوع | توضیح |
|---|---|---|
| `amount` | decimal | مبلغ خالص تومان — بدون کمیسیون (`Match.Price × EUR Amount`) |
| `invoiceNumber` | string | کد رهگیری تراکنش — مثال: `TX-ABC123-XYZ` |
| `expireAt` | datetime (UTC) | مهلت پرداخت — ۲۴ ساعت از زمان ایجاد تراکنش |

---

## فرمول محاسبه مبالغ

```
baseToman           = Match.Price × EUR Amount
sender.amount       = baseToman × (1 + ReceiverCommissionPercent / 100)
receiver.amount     = baseToman × (1 − SenderCommissionPercent / 100)
```

### مثال

| پارامتر | مقدار |
|---|---|
| Match.Price | 1,000 تومان |
| EUR Amount | 100 |
| SenderCommissionPercent (تیر ارسال‌کننده یورو) | 2% |
| ReceiverCommissionPercent (تیر دریافت‌کننده یورو) | 1% |

```
baseToman        = 1000 × 100 = 100,000
sender.amount    = 100,000 × 1.01 = 101,000  ← پرداخت می‌کند
receiver.amount  = 100,000 × 0.98 =  98,000  ← دریافت می‌کند
```

---

## خطاها

| کد | پیام | دلیل |
|---|---|---|
| `404` | Match not found | match با این id وجود ندارد |
| `400` | Match is not confirmed by both parties | status هنوز Active نیست |
| `400` | Transaction not created yet | هر دو تایید کرده‌اند اما transaction ایجاد نشده |
| `400` | Sender request has no receiver info | درخواست Send فاقد Receiver است |
| `403` | Not a participant of this match | کاربر جزء این match نیست و Admin هم نیست |
