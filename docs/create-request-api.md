# POST /api/Requests — ثبت درخواست

## Endpoint

```
POST /api/Requests
Authorization: Bearer <token>
Content-Type: application/json
```

---

## Enums

### type
| مقدار | توضیح |
|---|---|
| `0` | Send — ارسال ارز |
| `1` | Receive — دریافت ارز |

### currency
| مقدار | توضیح |
|---|---|
| `0` | EUR |
| `1` | USD |
| `2` | CAD |

### rateType
| مقدار | توضیح |
|---|---|
| `0` | Market — نرخ بازار |
| `1` | Instant — نرخ فوری |
| `2` | Custom — نرخ دستی (باید `customRate` پر باشد) |

### paymentMethods
| مقدار | توضیح |
|---|---|
| `0` | Revolut |
| `1` | Zelle |
| `2` | PayPal |
| `3` | SEPA |
| `4` | Wire |

---

## فیلدهای مشترک (هر دو حالت)

| فیلد | نوع | اجباری | توضیح |
|---|---|---|---|
| `type` | `int` | بله | نوع درخواست |
| `currency` | `int` | بله | نوع ارز |
| `amount` | `decimal` | بله | مبلغ — باید بزرگ‌تر از صفر باشد |
| `rateType` | `int` | بله | نوع نرخ |
| `customRate` | `decimal \| null` | فقط وقتی `rateType=2` | نرخ دستی |
| `paymentMethods` | `int[]` | بله | حداقل یک روش پرداخت |

---

## حالت Send (`type: 0`)

### فیلدهای اضافه

| فیلد | نوع | اجباری | توضیح |
|---|---|---|---|
| `receiverId` | `uuid` | بله | ID گیرنده ایرانی از `GET /api/Receivers` |

### مثال

```json
{
  "type": 0,
  "currency": 1,
  "amount": 200.0,
  "rateType": 0,
  "customRate": null,
  "paymentMethods": [0, 3],
  "receiverId": "19508989-b374-445c-8e39-46527c24fede"
}
```

---

## حالت Receive (`type: 1`)

### فیلدهای اضافه

| فیلد | نوع | اجباری | توضیح |
|---|---|---|---|
| `tomanPayer` | `object` | بله | اطلاعات پرداخت‌کننده تومان |
| `foreignAccounts` | `object[]` | بله | حداقل یک حساب خارجی |

### ساختار `tomanPayer`

اطلاعات شخصی که تومان را پرداخت می‌کند (ارسال‌کننده داخلی).

| فیلد | نوع | اجباری | توضیح |
|---|---|---|---|
| `fullName` | `string` | بله | نام و نام خانوادگی |
| `mobileNumber` | `string` | بله | شماره موبایل |

### ساختار هر آیتم در `foreignAccounts`

#### فیلدهای مشترک همه روش‌ها

| فیلد | نوع | اجباری |
|---|---|---|
| `method` | `int` | بله |
| `fullName` | `string` | بله — نام صاحب حساب |

#### Revolut (`method: 0`)

| فیلد | نوع | اجباری |
|---|---|---|
| `username` | `string` | بله — آیدی یا شماره تلفن (`@user` یا `+1234...`) |
| `email` | `string \| null` | خیر |

#### Zelle (`method: 1`)

| فیلد | نوع | اجباری |
|---|---|---|
| `emailOrPhone` | `string` | بله |

#### PayPal (`method: 2`)

| فیلد | نوع | اجباری |
|---|---|---|
| `email` | `string` | بله |

#### SEPA (`method: 3`)

| فیلد | نوع | اجباری |
|---|---|---|
| `iban` | `string` | بله |
| `bic` | `string` | بله — کد BIC/SWIFT |
| `bankName` | `string` | بله |

#### Wire (`method: 4`)

| فیلد | نوع | اجباری |
|---|---|---|
| `accountNum` | `string` | بله |
| `swift` | `string` | بله — کد SWIFT/BIC |
| `bankName` | `string` | بله |
| `bankAddress` | `string \| null` | خیر |

### مثال — یک روش پرداخت

```json
{
  "type": 1,
  "currency": 0,
  "amount": 500.0,
  "pricePreference": 0,
  "paymentMethods": [0],
  "tomanPayer": {
    "fullName": "علی محمدی",

    "mobileNumber": "09121234567"
  },
  "foreignAccounts": [
    {
      "method": 0,
      "fullName": "John Doe",
      "username": "@johndoe",
      "email": null
    }
  ]
}
```

### مثال — چند روش پرداخت

```json
{
  "type": 1,
  "currency": 0,
  "amount": 500.0,
  "pricePreference": 1,
  "paymentMethods": [0, 1, 3],
  "tomanPayer": {
    "fullName": "علی محمدی",

    "mobileNumber": "09121234567"
  },
  "foreignAccounts": [
    {
      "method": 0,
      "fullName": "John Doe",
      "username": "@johndoe",
      "email": "john@example.com"
    },
    {
      "method": 1,
      "fullName": "John Doe",
      "emailOrPhone": "john@example.com"
    },
    {
      "method": 3,
      "fullName": "John Doe",
      "iban": "GB29NWBK60161331926819",
      "bic": "NWBKGB2L",
      "bankName": "Barclays Bank"
    }
  ]
}
```

---

## پاسخ موفق (ثبت درخواست)

```
HTTP 201 Created
```

```json
{ "id": "<uuid>" }
```

---

## GET /api/Requests/{id} — جزئیات درخواست

پاسخ برای درخواست‌های `type: 1` (Receive) شامل `tomanPayer` می‌شود:

```json
{
  "id": "uuid",
  "type": 1,
  "currency": 0,
  "amount": 500.0,
  "pricePreference": 0,
  "paymentMethods": ["Revolut"],
  "status": 0,
  "expiresAt": "...",
  "createdAt": "...",
  "receiverId": null,
  "tomanPayer": {
    "fullName": "علی محمدی",

    "mobileNumber": "09121234567"
  },
  "foreignAccounts": [...]
}
```

برای درخواست‌های `type: 0` (Send)، فیلد `tomanPayer` برابر `null` است.
