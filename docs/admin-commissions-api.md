# Commission Management API

Admin endpoints for managing per-tier sender/receiver commissions.  
All endpoints require `Authorization: Bearer <admin-token>` (role: `Admin`).

---

## GET /api/commissions

Returns all configured tier commissions, ordered by tier level.

### Response `200 OK`

```json
[
  {
    "tierCommissionId": "uuid",
    "tierId": "11111111-1111-1111-1111-111111111111",
    "tierName": "Bronze",
    "tierOrder": 1,
    "senderCommissionPercent": 2.00,
    "receiverCommissionPercent": 1.50
  },
  {
    "tierCommissionId": "uuid",
    "tierId": "22222222-2222-2222-2222-222222222222",
    "tierName": "Silver",
    "tierOrder": 2,
    "senderCommissionPercent": 1.50,
    "receiverCommissionPercent": 1.00
  },
  {
    "tierCommissionId": "uuid",
    "tierId": "33333333-3333-3333-3333-333333333333",
    "tierName": "Gold",
    "tierOrder": 3,
    "senderCommissionPercent": 0.70,
    "receiverCommissionPercent": 0.50
  }
]
```

> If a tier has no commission set yet, it does not appear in this list.

---

## PUT /api/commissions/{tierId}

Create or update commission rates for a specific tier (upsert).  
Commission values are percentages (e.g. `2.00` = 2%).  
Applies to **all currencies** — no per-currency distinction.

### Path Parameter

| Name   | Type | Description         |
|--------|------|---------------------|
| tierId | guid | ID of the tier      |

### Request Body

```json
{
  "senderCommissionPercent": 2.00,
  "receiverCommissionPercent": 1.50
}
```

| Field                      | Type    | Required | Description                          |
|----------------------------|---------|----------|--------------------------------------|
| senderCommissionPercent    | decimal | yes      | Commission % charged to sender       |
| receiverCommissionPercent  | decimal | yes      | Commission % charged to receiver     |

### Response `204 No Content`

Success — no body returned.

### Response `404 Not Found`

```json
{
  "message": "Tier '{tierId}' not found."
}
```

---

## Tier IDs (seeded defaults)

| Tier   | ID                                   | Order |
|--------|--------------------------------------|-------|
| Bronze | `11111111-1111-1111-1111-111111111111` | 1 |
| Silver | `22222222-2222-2222-2222-222222222222` | 2 |
| Gold   | `33333333-3333-3333-3333-333333333333` | 3 |

---

## Example: Set Bronze tier commissions

```http
PUT /api/commissions/11111111-1111-1111-1111-111111111111
Authorization: Bearer <admin-token>
Content-Type: application/json

{
  "senderCommissionPercent": 2.00,
  "receiverCommissionPercent": 1.50
}
```

## Example: Set Gold tier commissions

```http
PUT /api/commissions/33333333-3333-3333-3333-333333333333
Authorization: Bearer <admin-token>
Content-Type: application/json

{
  "senderCommissionPercent": 0.70,
  "receiverCommissionPercent": 0.50
}
```

---

## Notes

- Commission is **per tier**, not per currency. Same rate applies to EUR, USD, CAD.
- Calling `PUT` on a tier that already has commission → updates existing record.
- Calling `PUT` on a tier with no commission yet → creates new record.
- `GET /api/tiers` returns tier list (limits, thresholds). Use `GET /api/commissions` for commission rates.
