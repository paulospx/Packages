Here are several useful SQL queries depending on exactly what you mean by "a certain price or the closest one":

### 1. Most common version: Find offers with price **exactly equal** OR **closest possible** to target price

```sql
WITH target AS (
    SELECT 99.99 AS target_price   -- ← change this to your desired price
),
diffs AS (
    SELECT 
        *,
        ABS(price - (SELECT target_price FROM target)) AS price_diff
    FROM offers
)
SELECT 
    *,
    price_diff
FROM diffs
WHERE price_diff = (SELECT MIN(price_diff) FROM diffs)
ORDER BY 
    price ASC,     -- if multiple offers have same difference → prefer cheaper
    id;            -- or whatever your tie-breaker column is
```

### 2. Simpler version (good performance, PostgreSQL/MySQL 8+/SQL Server)

```sql
SELECT *
FROM offers
ORDER BY ABS(price - 99.99) ASC,   -- ← your target price here
         price ASC                 -- secondary sort: cheaper first when distances equal
LIMIT 5;                           -- or 1, 3, 10... depending on how many close matches you want
```

### 3. Want **exactly the price** if exists, otherwise the closest?

```sql
-- PostgreSQL / MySQL 8+ / SQL Server
WITH target AS (SELECT 99.99 AS wanted),
exact AS (
    SELECT *, 0 AS distance
    FROM offers 
    WHERE price = (SELECT wanted FROM target)
),
closest AS (
    SELECT *, ABS(price - (SELECT wanted FROM target)) AS distance
    FROM offers
    WHERE price != (SELECT wanted FROM target)
)
SELECT * FROM exact
UNION ALL
SELECT * FROM closest
WHERE distance = (SELECT MIN(distance) FROM closest)
ORDER BY distance, price
LIMIT 10;
```

### Quick recommendation – most universal & practical:

```sql
-- Most universal solution - works everywhere
SELECT 
    id,
    price,
    name,
    ABS(price - :target_price) AS distance
FROM offers
ORDER BY 
    distance ASC,
    price ASC          -- when same distance → prefer lower price
LIMIT 5;               -- adjust as needed
```

Just replace `:target_price` with your value (or use `?` / named parameter depending on your programming language).

Pick version 2 or the last one for most real-world applications — they're simple, performant, and give reasonable results in 95% of cases.