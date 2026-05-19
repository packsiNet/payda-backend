# Frontend Changes Required: KYC Phone Verification via Telegram shareContact

## Context

The backend KYC flow has been updated. Phone number is **no longer entered manually** by the user.
Instead, it must be verified via Telegram's `requestContact()` (shareContact button) **before** submitting KYC.

---

## Backend API Changes

### 1. New Endpoint — Verify Phone Number
```
POST /api/users/me/phone
Authorization: Bearer <token>
Content-Type: application/json

{ "phoneNumber": "+98XXXXXXXXXX" }
```
- Returns `204 No Content` on success.
- Call this after the user shares their contact via Telegram.

### 2. Updated KYC Endpoint — Phone field removed
```
POST /api/users/me/kyc
Authorization: Bearer <token>
Content-Type: multipart/form-data

firstName: string
lastName: string
dateOfBirth: string   (format: YYYY-MM-DD)
selfieImage: File
documentImage: File
```
- `phoneNumber` is **no longer part of this request**.
- If phone is not verified first, the server returns an error.

---

## Frontend Tasks

### Step 1 — Add "Verify Phone Number" button
- Show a button labeled **"تأیید شماره موبایل"** (or "Verify Phone Number").
- On tap, call Telegram Web App's contact request:
  ```js
  WebApp.requestContact((success, result) => {
    if (success) {
      const phone = result.contact.phone_number;
      // normalize: ensure it starts with +
      const normalized = phone.startsWith('+') ? phone : '+' + phone;
      await api.post('/api/users/me/phone', { phoneNumber: normalized });
    }
  });
  ```
- After success, mark phone as verified in local state.

### Step 2 — Update KYC form
- **Remove** the phone number input field from the KYC form entirely.
- Show verified phone (read-only) fetched from `GET /api/users/me` if already set.
- **Disable** the KYC submit button until phone is verified.

### Step 3 — UX flow order
```
[Verify Phone Number button]  ──►  POST /api/users/me/phone
         ↓ (verified)
[KYC Form: firstName, lastName, dateOfBirth, selfie, document]
         ↓
                              ──►  POST /api/users/me/kyc
```

---

## Notes
- `PhoneNumber` shown in profile comes from the verified contact, not from KYC form input.
- If the user already has a phone number on their profile (`GET /api/users/me` returns `phoneNumber != null`), skip the verify step and go straight to KYC form.
