Got it 👍

With **22M rows** in SQL Server (MSSQL), you’ll want to design indexes carefully, otherwise you risk making writes slower or bloating storage. Since your data can be split by `FISCAL_YEAR`, you have two main options depending on your query patterns:

---

## 1. **Partitioning by Fiscal Year** (Best for very large tables)

If queries are almost always filtered by `FISCAL_YEAR`, consider **table partitioning**. That way SQL Server only scans relevant partitions (e.g., one year at a time).

Example (range partitioning on `FISCAL_YEAR`):

```sql
-- Step 1: Create Partition Function
CREATE PARTITION FUNCTION pf_FiscalYearRange (INT)
AS RANGE LEFT FOR VALUES (2018, 2019, 2020, 2021, 2022, 2023);

-- Step 2: Create Partition Scheme
CREATE PARTITION SCHEME ps_FiscalYearRange
AS PARTITION pf_FiscalYearRange
ALL TO ([PRIMARY]);

-- Step 3: Create Partitioned Table (if redesigning table)
-- Otherwise, you can switch your existing table to this scheme
CREATE TABLE dbo.YourTable
(
    Id BIGINT IDENTITY(1,1) NOT NULL,
    Fiscal_Year INT NOT NULL,
    Col1 ...,
    Col2 ...,
    ...
)
ON ps_FiscalYearRange(Fiscal_Year);
```

Partitioning makes queries like:

```sql
SELECT * FROM YourTable WHERE Fiscal_Year = 2022;
```

only scan the partition for 2022 instead of the whole 22M rows.

---

## 2. **Filtered Indexes** (If partitioning is overkill)

If you mostly query one or two fiscal years at a time, filtered indexes can be much lighter:

```sql
-- Example: Index for 2023 queries
CREATE NONCLUSTERED INDEX IX_YourTable_FY2023
ON dbo.YourTable (Col1, Col2)  -- choose columns often filtered/sorted
WHERE Fiscal_Year = 2023;
```

You can add filtered indexes only for active/current years to keep maintenance low.

---

## 3. **General Index Recommendation**

At minimum, if `FISCAL_YEAR` is used in most filters/joins, you should have:

```sql
CREATE NONCLUSTERED INDEX IX_YourTable_FiscalYear
ON dbo.YourTable (Fiscal_Year)
INCLUDE (Col1, Col2); -- columns frequently SELECTed
```

This lets SQL Server do an index seek instead of a full table scan.
If you often sort by `FISCAL_YEAR`, make it the **leading key** of your index.

---

✅ **Summary**

* Use **partitioning** if queries are year-based and the dataset keeps growing.
* Use **filtered indexes** if you only need fast access to *recent years*.
* Always **tailor INCLUDE columns** to match the query’s `SELECT` list to avoid lookups.

---

👉 Can you share the **most common query patterns** (e.g., always filtering by `Fiscal_Year` + `Customer_Id`, or aggregating per year)? That way I can generate the *exact* index script for your workload.
