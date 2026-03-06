As an expert in usability and information design (inspired by Nielsen Norman Group principles), the goal is to make the **comparison report clear, scannable, prioritized by user needs**, and focused on helping the reader quickly understand **what changed** between M01 and M02, why it might matter, and where to drill down.

### Recommended Report Structure (Yield Curve Comparison – M01 vs M02)

1. **Executive Summary (top – most important)**
   - Key facts in 4–6 bullet points or small cards:
     - Number of curves only in M01 (disappeared)
     - Number of curves only in M02 (new)
     - Number of common curves
     - Largest absolute change (yield ↑ or ↓) + which curve + at which maturity
     - Largest relative change (%)
     - Overall average shift direction (parallel shift up/down, steepening/flattening)

2. **Overview Panel – Status of Curves**
   Use a clean table (sortable if interactive):

   | Status       | Count | Curve Names (example)                  | Actionable note                  |
   |--------------|-------|----------------------------------------|----------------------------------|
   | Common       | 38    | EUR_3M, USD_10Y, ...                   | Detailed diff available          |
   | Only in M01  | 7     | OLD_SWAP_5Y, ...                       | Removed / no longer published?   |
   | Only in M02  | 4     | NEW_CORP_BBB_7Y, ...                   | Newly added                      |

3. **Biggest Differences – Highlight & Prioritize (core value)**
   - **Top 10 Absolute Differences** table (sorted descending by |diff|)
   - **Top 10 Relative Differences** (%) table (useful when yields are low)

   Example columns for both tables:

   | Rank | Curve Name     | Maturity | M01 Yield (%) | M02 Yield (%) | Diff (bp) | Diff (%) | Color flag          |
   |------|----------------|----------|---------------|---------------|-----------|----------|---------------------|
   | 1    | USD_SWAP_2Y    | 2Y       | 3.12          | 4.85          | +173      | +55.4%   | <span style="color:red">↑↑</span> |

   Use conditional formatting:
   - Red background / ↑ arrows for large positive diffs (yields up = borrowing more expensive)
   - Green / ↓ for large negative
   - Thresholds e.g. > |50 bp| or > |10%| get strong highlighting

4. **Visual Comparison – Common Curves**
   Two strongly recommended chart types (combine both):

   **A. Overlaid Line Chart – Classic & Most Intuitive**
   - X-axis = Maturity (standardized: 1M, 3M, 6M, 1Y, 2Y, 5Y, 7Y, 10Y, 20Y, 30Y, … or numerical days/months)
   - Y-axis = Yield (%)
   - Two lines: M01 (e.g. blue dashed), M02 (solid orange)
   - Optional: fill area between curves (shaded gray or red/green depending on direction)

   **B. Difference Chart (very powerful for spotting shape changes)**
   - Same X-axis (maturities)
   - Y-axis = Change in yield (bp) = M02 – M01
   - Horizontal line at 0
   - Bars or area chart
   - Positive bars = yields increased
   - Negative = decreased
   - This immediately reveals **parallel shifts**, **steepening**, **flattening**, **humps**, **twists**

   For many curves: either
   - One chart with top 8–12 most changed curves overlaid
   - Small multiples (facet grid): one subplot per curve (good for 20–40 curves)

5. **Detailed Table – All Common Curves (appendix / expandable section)**
   - One row per curve + maturity combination (long format is better than wide)
   - Columns: Curve, Maturity, M01 (%), M02 (%), Diff (bp), Diff (%), Abs Diff (bp)

   Sortable by any column, especially by Abs Diff descending.

6. **Optional Advanced Visuals (if needed)**
   - Heatmap: rows = curves, columns = maturities, color = diff in bp (red–blue diverging)
   - Waterfall chart for selected curves: contribution of short/mid/long end to total shift

This structure follows NN/g heuristics:
- **Information scent** — biggest issues first
- **Progressive disclosure** — summary → highlights → visuals → details
- **Low cognitive load** — consistent coloring, clear labels, avoid chartjunk

### Python Code to Generate Such a Report

Assumes:
- `df1` = M01 (pandas DataFrame), columns = curve names, index = maturities (as strings or floats)
- `df2` = M02, same structure
- Maturing ordering should be consistent if possible

```python
import pandas as pd
import numpy as np
import matplotlib.pyplot as plt
import seaborn as sns

# ── 1. Prepare comparison data ──────────────────────────────────────────────
common_curves = df1.columns.intersection(df2.columns)
only_m01      = df1.columns.difference(df2.columns)
only_m02      = df2.columns.difference(df1.columns)

print(f"Common curves : {len(common_curves)}")
print(f"Only M01      : {len(only_m01)} → {list(only_m01)[:5]}...")
print(f"Only M02      : {len(only_m02)} → {list(only_m02)[:5]}...")

# Long format diffs for common curves
diffs = []
for curve in common_curves:
    s1 = df1[curve].dropna()
    s2 = df2[curve].dropna()
    mat = s1.index.intersection(s2.index)          # only shared maturities
    if len(mat) == 0:
        continue
    d = pd.DataFrame({
        'curve': curve,
        'maturity': mat,
        'M01': s1.loc[mat],
        'M02': s2.loc[mat],
    })
    d['diff_bp']  = (d['M02'] - d['M01']) * 100     # assuming yields in decimal
    d['diff_pct'] = np.where(d['M01'] != 0, d['diff_bp'] / (d['M01']*100), np.nan)
    d['abs_diff'] = d['diff_bp'].abs()
    diffs.append(d)

df_diff = pd.concat(diffs, ignore_index=True)

# ── 2. Biggest differences ──────────────────────────────────────────────────
print("\nTop 10 largest absolute changes (bp)")
print(df_diff.nlargest(10, 'abs_diff')[
    ['curve','maturity','M01','M02','diff_bp','diff_pct']
    .round(2)
])

print("\nTop 10 largest relative changes (%)")
print(df_diff.nlargest(10, 'diff_pct')[
    ['curve','maturity','M01','M02','diff_bp','diff_pct']
    .round(2)
])

# ── 3. Visuals ──────────────────────────────────────────────────────────────
sns.set_theme(style="whitegrid")

# A. Overlaid curves – example for top changed curves
top_curves = df_diff.groupby('curve')['abs_diff'].max().nlargest(8).index

plt.figure(figsize=(12, 7))
for curve in top_curves:
    sub = df_diff[df_diff['curve'] == curve].sort_values('maturity')
    plt.plot(sub['maturity'], sub['M01'], label=f"{curve} M01", linestyle='--', alpha=0.7)
    plt.plot(sub['maturity'], sub['M02'], label=f"{curve} M02", linewidth=2.2)

plt.title("Yield Curves Comparison – Top Changed Curves (M01 vs M02)")
plt.xlabel("Maturity")
plt.ylabel("Yield (%)")
plt.legend(bbox_to_anchor=(1.02, 1), loc='upper left')
plt.tight_layout()
plt.show()

# B. Difference bar chart – average or selected curves
# Example: average difference per maturity across all common curves
avg_diff = df_diff.groupby('maturity')['diff_bp'].mean().sort_index()

plt.figure(figsize=(10, 6))
avg_diff.plot(kind='bar', color=np.where(avg_diff > 0, 'salmon', 'lightgreen'))
plt.axhline(0, color='gray', linestyle='--', linewidth=1.2)
plt.title("Average Yield Change by Maturity (M02 – M01)")
plt.ylabel("Change in bp")
plt.xlabel("Maturity")
plt.xticks(rotation=45)
plt.tight_layout()
plt.show()

# C. Optional: difference lines for many curves (small multiples)
g = sns.FacetGrid(df_diff, col='curve', col_wrap=4, height=3, aspect=1.3, sharey=False)
g.map(sns.lineplot, 'maturity', 'diff_bp')
g.map(plt.axhline, y=0, color='gray', ls='--')
g.set_titles("{col_name}")
g.set_axis_labels("Maturity", "Diff (bp)")
g.figure.suptitle("Yield Change per Curve (M02 – M01)", y=1.02)
plt.show()
```

Adapt maturities sorting, units (decimal vs percent), colors, and filtering as needed. For production reports consider Plotly (interactive) or Jupyter + nbconvert → HTML/PDF.

This gives a scannable, actionable report focused on **what changed most** — exactly what busy stakeholders usually need. Let me know if your maturities are numeric or you want a Streamlit/Panel version!
