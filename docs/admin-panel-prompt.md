# PayDa Admin Panel — Frontend Build Prompt

## Overview

Build a **web-based admin panel** for the PayDa currency exchange platform. The backend is an ASP.NET Core REST API with JWT Bearer authentication. The admin panel is used by users with roles `Admin` or `Agent`.

**Base URL:** `https://<your-api-host>/api`  
**Auth:** All requests (except login) require header:
```
Authorization: Bearer <jwt_token>
```

---

## Enums (Shared Types)

```typescript
enum Currency       { EUR = 0, USD = 1, CAD = 2 }
enum KycStatus      { NotSubmitted = 0, Pending = 1, Approved = 2, Rejected = 3 }
enum UserRole       { User = 0, Agent = 1, Admin = 2 }
enum RequestType    { Send = 0, Receive = 1 }
enum RequestStatus  { Pending = 0, Matched = 1, Expired = 2, Cancelled = 3 }
enum RateType       { Market = 0, Instant = 1, Custom = 2 }
enum PaymentMethod  { Revolut = 0, Zelle = 1, PayPal = 2, SEPA = 3, Wire = 4 }
enum TransactionStatus { Pending = 0, ScreenshotUploaded = 1, Confirmed = 2, Settled = 3, Disputed = 4 }
enum MatchStatus    { Active = 0, Completed = 1, Cancelled = 2 }
```

---

## Authentication

### POST `/api/auth/telegram-login`
ورود از طریق Telegram WebApp

**Request Body:**
```json
{
  "initData": "string"  // داده‌های initData از Telegram WebApp
}
```

**Response `200`:**
```json
{
  "token": "string",        // JWT token برای استفاده در header
  "isNewUser": true,
  "kycStatus": 0            // KycStatus enum
}
```

---

## Users (مدیریت کاربران)

### GET `/api/users`
**[Admin Only]** لیست تمام کاربران با صفحه‌بندی

**Query Params:**
| پارامتر   | نوع   | پیش‌فرض | توضیح             |
|-----------|-------|---------|-------------------|
| `page`    | int   | 1       | شماره صفحه        |
| `pageSize`| int   | 20      | تعداد در هر صفحه  |

**Response `200`:**
```json
[
  {
    "id": "guid",
    "telegramId": 123456789,
    "telegramUsername": "string | null",
    "firstName": "string | null",
    "lastName": "string | null",
    "kycStatus": 0,          // KycStatus enum
    "role": 0,               // UserRole enum
    "isTrusted": false,
    "tierName": "string",
    "createdAt": "datetime"
  }
]
```

---

### GET `/api/users/me`
**[Authorized]** پروفایل کاربر جاری

**Response `200`:**
```json
{
  "id": "guid",
  "telegramId": 123456789,
  "telegramUsername": "string | null",
  "firstName": "string | null",
  "lastName": "string | null",
  "kycStatus": 0,
  "role": 0,
  "isTrusted": false,
  "tierName": "string",
  "tierOrder": 1,
  "completedTransactionsCount": 5,
  "phoneVerified": false,
  "selfieImageUrl": "string | null",
  "documentImageUrl": "string | null"
}
```

---

### POST `/api/users/me/phone`
**[Authorized]** ثبت شماره تلفن

**Request Body:**
```json
{ "phoneNumber": "string" }
```

**Response `204 No Content`**

---

### GET `/api/users/me/kyc`
**[Authorized]** وضعیت KYC کاربر جاری

**Response `200`:**
```json
{
  "status": 1,              // KycStatus enum
  "displayName": "Pending"
}
```

---

### POST `/api/users/me/kyc`
**[Authorized]** ارسال مدارک KYC

**Request:** `multipart/form-data`

| فیلد            | نوع     | توضیح              |
|-----------------|---------|--------------------|
| `firstName`     | string  | نام                |
| `lastName`      | string  | نام خانوادگی       |
| `dateOfBirth`   | string  | تاریخ تولد (YYYY-MM-DD) |
| `selfieImage`   | file    | عکس سلفی           |
| `documentImage` | file    | عکس مدرک هویتی    |

**Response `204 No Content`**

---

### POST `/api/users/{id}/approve-kyc`
**[Admin Only]** تایید KYC کاربر

**Path Param:** `id` (guid)  
**Response `204 No Content`**

---

### POST `/api/users/{id}/reject-kyc`
**[Admin Only]** رد KYC کاربر

**Path Param:** `id` (guid)  
**Response `204 No Content`**

---

### POST `/api/users/{id}/trusted`
**[Admin Only]** تغییر وضعیت Trusted کاربر

**Path Param:** `id` (guid)

**Request Body:**
```json
{ "isTrusted": true }
```

**Response `204 No Content`**

---

### POST `/api/users/{id}/role`
**[Admin Only]** تغییر نقش کاربر

**Path Param:** `id` (guid)

**Request Body:**
```json
{ "role": 1 }   // UserRole enum: User=0, Agent=1, Admin=2
```

**Response `204 No Content`**

---

## Exchange Rates (نرخ ارز)

### GET `/api/exchangerates`
**[Authorized]** دریافت نرخ‌های ارز فعلی

**Response `200`:**
```json
[
  {
    "currency": 0,          // Currency enum: EUR=0, USD=1, CAD=2
    "marketRate": 65000.0,
    "instantRate": 64500.0,
    "updatedAt": "datetime | null"
  }
]
```

---

### PUT `/api/exchangerates`
**[Admin Only]** به‌روزرسانی نرخ ارز

**Request Body:**
```json
{
  "currency": 0,            // Currency enum
  "marketRate": 65000.0,
  "instantRate": 64500.0
}
```

**Response `204 No Content`**

---

## Tiers (سطح‌بندی کاربران)

### GET `/api/tiers`
**[Authorized]** لیست تمام تیرها

**Response `200`:**
```json
[
  {
    "id": "guid",
    "name": "Bronze",
    "order": 1,
    "maxActiveRequests": 3,
    "maxAmountPerRequest": 1000.00,
    "requiredCompletedTransactions": 0
  }
]
```

---

### POST `/api/tiers`
**[Admin Only]** ایجاد تیر جدید

**Request Body:**
```json
{
  "name": "string",
  "order": 1,
  "maxActiveRequests": 3,
  "maxAmountPerRequest": 1000.00,
  "requiredCompletedTransactions": 0
}
```

**Response `201 Created`:**
```json
{ "id": "guid" }
```

---

### PUT `/api/tiers/{id}`
**[Admin Only]** به‌روزرسانی تیر

**Path Param:** `id` (guid)

**Request Body:**
```json
{
  "name": "string",
  "maxActiveRequests": 5,
  "maxAmountPerRequest": 2000.00,
  "requiredCompletedTransactions": 10
}
```

**Response `204 No Content`**

---

## Matches (مچ درخواست‌ها)

### GET `/api/matches/my`
**[Authorized]** لیست مچ‌های کاربر جاری

**Response `200`:**
```json
[
  {
    "matchId": "guid",
    "myRequestId": "guid",
    "myRequestType": 0,       // RequestType: Send=0, Receive=1
    "currency": 0,
    "amount": 500.00,
    "rateValue": 64500.0,
    "expiresAt": "datetime",
    "requestDate": "datetime",
    "matchDate": "datetime",
    "counterpartDisplayName": "string",
    "counterpartLevel": 1,
    "counterpartLevelTitle": "Bronze",
    "counterpartIsTrusted": false,
    "counterpartPaymentMethods": ["Revolut", "SEPA"],
    "transactionId": "guid",
    "transactionStatus": 0     // TransactionStatus enum
  }
]
```

---

### POST `/api/matches`
**[Authorized]** مچ کردن درخواست توسط کاربر (انتخاب طرف مقابل)

**Request Body:**
```json
{ "requestId": "guid" }   // آی‌دی درخواست طرف مقابل
```

**Response `200`:**
```json
{
  "matchId": "guid",
  "message": "string"
}
```

---

### POST `/api/matches/admin`
**[Admin, Agent]** مچ دستی توسط ادمین یا عامل

**Request Body:**
```json
{
  "senderRequestId": "guid",
  "receiverRequestId": "guid",
  "isAgentInvolved": false
}
```

**Response `200`:**
```json
{ "id": "guid" }
```

---

## Requests (درخواست‌های صرافی)

### GET `/api/requests/search`
**[Authorized]** جستجوی درخواست‌های مطابق

**Query Params:**
| پارامتر    | نوع          | توضیح                              |
|------------|-------------|-------------------------------------|
| `type`     | RequestType | نوع درخواست: Send=0, Receive=1     |
| `currency` | Currency    | نوع ارز: EUR=0, USD=1, CAD=2      |
| `amount`   | decimal     | مقدار                              |

**Response `200`:**
```json
[
  {
    "requestId": "guid",
    "userInitials": "AB",
    "userDisplayName": "Ali B.",
    "userLevel": 1,
    "userLevelTitle": "Bronze",
    "isTrusted": false,
    "amount": 500.00,
    "rateValue": 64500.0,
    "paymentMethods": ["Revolut"],
    "createdAt": "datetime"
  }
]
```

---

### GET `/api/requests`
**[Authorized]** لیست درخواست‌های کاربر جاری

**Query Params:**
| پارامتر | نوع         | توضیح                           |
|---------|------------|----------------------------------|
| `type`  | RequestType? | فیلتر بر اساس نوع (اختیاری)  |

**Response `200`:**
```json
[
  {
    "id": "guid",
    "type": 0,               // RequestType
    "currency": 0,           // Currency
    "amount": 500.00,
    "rateValue": 64500.0,
    "paymentMethods": ["Revolut"],
    "status": 0,             // RequestStatus
    "expiresAt": "datetime",
    "createdAt": "datetime",
    "ownerAvatarInitials": "AB",
    "ownerProfilePhotoUrl": "string | null",
    "ownerDisplayName": "Ali B.",
    "ownerIsTrusted": false,
    "ownerTierName": "Bronze",
    "ownerTierOrder": 1
  }
]
```

---

### GET `/api/requests/{id}`
**[Authorized]** جزئیات کامل یک درخواست

**Path Param:** `id` (guid)

**Response `200`:**
```json
{
  "id": "guid",
  "type": 0,
  "currency": 0,
  "amount": 500.00,
  "rateType": 0,           // RateType: Market=0, Instant=1, Custom=2
  "rateValue": 64500.0,
  "commissionPercent": 1.5,
  "commissionAmount": 7.5,
  "paymentMethods": ["Revolut"],
  "status": 0,
  "expiresAt": "datetime",
  "createdAt": "datetime",
  "receiverId": "guid | null",
  "foreignAccounts": [
    {
      "id": "guid",
      "method": 0,         // PaymentMethod enum
      "fullName": "string",
      "username": "string | null",
      "email": "string | null",
      "emailOrPhone": "string | null",
      "iban": "string | null",
      "bic": "string | null",
      "bankName": "string | null",
      "accountNum": "string | null",
      "swift": "string | null",
      "bankAddress": "string | null"
    }
  ]
}
```

---

### POST `/api/requests/preview`
**[Authorized]** پیش‌نمایش محاسبات قبل از ثبت درخواست

**Request Body:**
```json
{
  "type": 0,               // RequestType
  "currency": 0,           // Currency
  "amount": 500.00,
  "rateType": 0,           // RateType
  "customRate": null       // decimal | null (فقط برای RateType.Custom)
}
```

**Response `200`:**
```json
{
  "amount": 500.00,
  "rateValue": 64500.0,
  "commissionPercent": 1.5,
  "commissionAmount": 7.5,
  "totalAmount": 507.5
}
```

---

### POST `/api/requests`
**[Authorized]** ثبت درخواست جدید

**Request Body:**
```json
{
  "type": 0,               // RequestType
  "currency": 0,           // Currency
  "amount": 500.00,
  "rateType": 0,           // RateType
  "customRate": null,      // decimal | null
  "paymentMethods": [0],   // آرایه‌ای از PaymentMethod enum
  "receiverId": null,      // guid | null — فقط برای Send
  "foreignAccounts": [     // فقط برای Receive
    {
      "method": 0,
      "fullName": "string",
      "username": "string | null",
      "email": "string | null",
      "emailOrPhone": "string | null",
      "iban": "string | null",
      "bic": "string | null",
      "bankName": "string | null",
      "accountNum": "string | null",
      "swift": "string | null",
      "bankAddress": "string | null"
    }
  ]
}
```

**Response `201 Created`:**
```json
{ "id": "guid" }
```

---

### DELETE `/api/requests/{id}`
**[Authorized]** لغو درخواست

**Path Param:** `id` (guid)  
**Response `204 No Content`**

---

## Transactions (تراکنش‌ها)

### GET `/api/transactions`
**[Authorized]** لیست تراکنش‌های کاربر جاری

**Query Params:**
| پارامتر    | نوع               | پیش‌فرض | توضیح               |
|------------|------------------|---------|----------------------|
| `type`     | RequestType?     | null    | فیلتر نوع (اختیاری)|
| `status`   | TransactionStatus?| null   | فیلتر وضعیت         |
| `page`     | int              | 1       | شماره صفحه          |
| `pageSize` | int              | 20      | تعداد در هر صفحه    |

**Response `200`:**
```json
[
  {
    "id": "guid",
    "matchId": "guid",
    "status": 0,             // TransactionStatus
    "referenceCode": "string | null",
    "myRole": 0,             // RequestType: Send=0, Receive=1
    "currency": 0,
    "amount": 500.00,
    "rateValue": 64500.0,
    "paymentMethod": "Revolut | null",
    "counterpartDisplayName": "string",
    "counterpartLevel": 1,
    "counterpartLevelTitle": "Bronze",
    "counterpartIsTrusted": false,
    "screenshotUrl": "string | null",
    "paidAt": "datetime | null",
    "settledAt": "datetime | null",
    "createdAt": "datetime"
  }
]
```

---

### GET `/api/transactions/{id}`
**[Authorized]** جزئیات کامل یک تراکنش

**Path Param:** `id` (guid)

**Response `200`:**
```json
{
  "id": "guid",
  "matchId": "guid",
  "status": 0,
  "screenshotUrl": "string | null",
  "paidAt": "datetime | null",
  "confirmedAt": "datetime | null",
  "settledAt": "datetime | null",
  "createdAt": "datetime"
}
```

---

### POST `/api/transactions/{id}/screenshot`
**[Authorized]** آپلود اسکرین‌شات پرداخت

**Path Param:** `id` (guid)  
**Request:** `multipart/form-data` با فیلد `file` (تصویر)  
**Response `204 No Content`**

---

### POST `/api/transactions/{id}/confirm`
**[Authorized]** تایید دریافت پرداخت توسط گیرنده

**Path Param:** `id` (guid)  
**Response `204 No Content`**

---

### POST `/api/transactions/{id}/settle`
**[Authorized]** تسویه نهایی تراکنش

**Path Param:** `id` (guid)  
**Response `204 No Content`**

---

## Receivers (حساب‌های دریافت ریالی)

### GET `/api/receivers`
**[Authorized]** لیست حساب‌های دریافت ریالی کاربر

**Response `200`:**
```json
[
  {
    "id": "guid",
    "firstName": "string",
    "lastName": "string",
    "nationalId": "string",
    "mobileNumber": "string",
    "iban": "string"
  }
]
```

---

### POST `/api/receivers`
**[Authorized]** افزودن حساب دریافت ریالی جدید

**Request Body:**
```json
{
  "firstName": "string",
  "lastName": "string",
  "nationalId": "string",
  "mobileNumber": "string",
  "iban": "string"
}
```

**Response `201 Created`:**
```json
{ "id": "guid" }
```

---

### DELETE `/api/receivers/{id}`
**[Authorized]** حذف حساب دریافت ریالی

**Path Param:** `id` (guid)  
**Response `204 No Content`**

---

## Admin Panel Requirements (نیازمندی‌های پنل ادمین)

### صفحات مورد نیاز

#### 1. داشبورد (Dashboard)
- آمار کلی: تعداد کاربران، درخواست‌های فعال، تراکنش‌های در انتظار
- لیست کاربران با KYC Pending برای بررسی سریع

#### 2. مدیریت کاربران (`/users`)
- جدول صفحه‌بندی‌شده کاربران (endpoint: `GET /api/users?page=&pageSize=`)
- نمایش: نام، یوزرنیم تلگرام، وضعیت KYC، نقش، Trusted، تیر، تاریخ ثبت
- فیلتر بر اساس: وضعیت KYC، نقش، Trusted
- اکشن‌ها روی هر کاربر:
  - تایید KYC (`POST /api/users/{id}/approve-kyc`)
  - رد KYC (`POST /api/users/{id}/reject-kyc`)
  - تغییر نقش (`POST /api/users/{id}/role`)
  - Toggle Trusted (`POST /api/users/{id}/trusted`)
- صفحه جزئیات کاربر: نمایش عکس سلفی و مدرک (از `selfieImageUrl` و `documentImageUrl`)

#### 3. مدیریت نرخ ارز (`/exchange-rates`)
- جدول نرخ‌های فعلی: EUR، USD، CAD
- نمایش `marketRate`، `instantRate`، `updatedAt`
- فرم ویرایش هر ارز (`PUT /api/exchangerates`)

#### 4. مدیریت تیرها (`/tiers`)
- لیست تیرها با ترتیب (order)
- فرم ایجاد تیر جدید (`POST /api/tiers`)
- فرم ویرایش تیر (`PUT /api/tiers/{id}`)
- فیلدها: name، order، maxActiveRequests، maxAmountPerRequest، requiredCompletedTransactions

#### 5. مچ دستی (`/admin-match`)
- **[Admin/Agent]**
- جستجو و انتخاب یک درخواست Send و یک درخواست Receive
- ثبت مچ دستی (`POST /api/matches/admin`)
- فیلد `isAgentInvolved` برای مشخص کردن دخالت عامل

#### 6. مدیریت درخواست‌ها (`/requests`) — اختیاری برای ادمین
- مشاهده همه درخواست‌های کاربران (در صورت نیاز API جدید اضافه شود)
- فعلاً: ادمین می‌تواند درخواست‌های هر کاربر را از پروفایل کاربر ببیند

---

## Error Responses

همه خطاها به فرمت زیر برگشت می‌دهند:

```json
{
  "type": "string",
  "title": "string",
  "status": 400,
  "detail": "string",
  "errors": {
    "fieldName": ["error message"]
  }
}
```

| HTTP Status | توضیح                                   |
|-------------|------------------------------------------|
| 400         | درخواست نامعتبر / Validation error       |
| 401         | احراز هویت نشده                          |
| 403         | دسترسی غیرمجاز (نقش کافی ندارد)         |
| 404         | منبع پیدا نشد                            |
| 500         | خطای داخلی سرور                          |

---

## Authorization Summary

| Endpoint                            | نقش مورد نیاز     |
|-------------------------------------|-------------------|
| `POST /api/auth/telegram-login`     | Public            |
| `GET /api/exchangerates`            | Any Authorized    |
| `GET /api/tiers`                    | Any Authorized    |
| `GET /api/users/me`                 | Any Authorized    |
| `GET /api/requests` (own)           | Any Authorized    |
| `GET /api/transactions` (own)       | Any Authorized    |
| `GET /api/matches/my`               | Any Authorized    |
| `GET /api/users` (all users)        | **Admin**         |
| `POST /api/users/{id}/approve-kyc`  | **Admin**         |
| `POST /api/users/{id}/reject-kyc`   | **Admin**         |
| `POST /api/users/{id}/trusted`      | **Admin**         |
| `POST /api/users/{id}/role`         | **Admin**         |
| `PUT /api/exchangerates`            | **Admin**         |
| `POST /api/tiers`                   | **Admin**         |
| `PUT /api/tiers/{id}`               | **Admin**         |
| `POST /api/matches/admin`           | **Admin, Agent**  |

---

## Tech Notes for Frontend

- تمام `id` ها از نوع `UUID/GUID` هستند
- تمام تاریخ‌ها به فرمت ISO 8601 (UTC) هستند: `"2025-01-15T10:30:00Z"`
- مقادیر enum به صورت عدد صحیح در API رد و بدل می‌شوند
- برای آپلود فایل از `multipart/form-data` استفاده شود
- JWT token باید در `localStorage` یا `sessionStorage` نگه‌داری شود
- در صورت دریافت `401`، کاربر به صفحه لاگین هدایت شود
