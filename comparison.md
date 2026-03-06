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


















Here is a realistic, practical Python script that:

- Compares two yield curve DataFrames
- Handles missing/new curves
- Creates summary statistics
- Produces the most important tables (top differences)
- Creates the two most valuable chart types
- Saves everything into a clean markdown report + images
- Produces a PDF via markdown → PDF conversion (using `pandoc` or fallback HTML)

```python
import pandas as pd
import numpy as np
import matplotlib.pyplot as plt
import seaborn as sns
from pathlib import Path
import datetime
import os

# ───────────────────────────────────────────────────────────────
#  CONFIGURATION
# ───────────────────────────────────────────────────────────────

REPORT_TITLE = "Yield Curve Comparison Report – M01 vs M02"
REPORT_DATE = datetime.date.today().strftime("%B %d, %Y")

# Folders
OUTPUT_DIR = Path("yield_curve_report")
OUTPUT_DIR.mkdir(exist_ok=True)

IMG_DIR = OUTPUT_DIR / "images"
IMG_DIR.mkdir(exist_ok=True)

MD_FILE = OUTPUT_DIR / "report.md"

# You should replace these with your real dataframes
# Example structure: index = maturities (e.g. '1M', '3M', '1Y', '2Y', ..., '30Y')
# columns = curve names

# ── Dummy data for demonstration ──
maturities = ['1M','3M','6M','1Y','2Y','3Y','5Y','7Y','10Y','20Y','30Y']

df_m01 = pd.DataFrame({
    'USD_SWAP':     [4.10, 4.05, 3.95, 3.80, 3.65, 3.55, 3.45, 3.50, 3.60, 3.75, 3.88],
    'EUR_SWAP':     [3.20, 3.15, 3.05, 2.90, 2.75, 2.60, 2.45, 2.55, 2.65, 2.80, 2.95],
    'BRL_DI_252':   [11.80,11.70,11.60,11.45,11.30,11.20,11.10,11.05,11.00,11.20,11.50],
    'OLD_CURVE_A':  [5.10, 5.00, 4.90, 4.80, 4.70, 4.60, 4.50, np.nan, np.nan, np.nan, np.nan],
}, index=maturities)

df_m02 = pd.DataFrame({
    'USD_SWAP':     [4.65, 4.70, 4.75, 4.80, 4.85, 4.70, 4.52, 4.45, 4.42, 4.55, 4.62],
    'EUR_SWAP':     [3.45, 3.50, 3.55, 3.60, 3.65, 3.55, 3.41, 3.35, 3.28, 3.40, 3.55],
    'BRL_DI_252':   [12.90,12.85,12.80,12.98,12.70,12.50,12.30,12.20,12.10,12.30,12.60],
    'NEW_CORP_BBB': [6.20, 6.15, 6.10, 6.05, 6.00, 5.95, 5.90, 5.85, 5.80, 5.90, 6.10],
}, index=maturities)

# ───────────────────────────────────────────────────────────────
#  1. Comparison logic
# ───────────────────────────────────────────────────────────────

common = df_m01.columns.intersection(df_m02.columns)
only_m01 = df_m01.columns.difference(df_m02.columns)
only_m02 = df_m02.columns.difference(df_m01.columns)

# ── Create long differences table ──
diff_rows = []

for curve in common:
    s1 = df_m01[curve].dropna()
    s2 = df_m02[curve].dropna()
    shared_matur = s1.index.intersection(s2.index)
    
    if len(shared_matur) == 0:
        continue
        
    d = pd.DataFrame({
        'curve': curve,
        'maturity': shared_matur,
        'yield_m01': s1[shared_matur],
        'yield_m02': s2[shared_matur],
    })
    d['diff_bp']   = (d['yield_m02'] - d['yield_m01']) * 100
    d['diff_pct']  = np.where(d['yield_m01'] != 0, d['diff_bp'] / (d['yield_m01'] * 100) * 100, np.nan)
    d['abs_diff']  = d['diff_bp'].abs()
    diff_rows.append(d)

df_diff = pd.concat(diff_rows, ignore_index=True)

# ───────────────────────────────────────────────────────────────
#  2. Save top tables
# ───────────────────────────────────────────────────────────────

top_abs = df_diff.nlargest(10, 'abs_diff').copy()
top_abs = top_abs[['curve','maturity','yield_m01','yield_m02','diff_bp','diff_pct']].round(2)

top_rel = df_diff.nlargest(10, 'diff_pct').copy()
top_rel = top_rel[['curve','maturity','yield_m01','yield_m02','diff_bp','diff_pct']].round(2)

# ───────────────────────────────────────────────────────────────
#  3. Charts
# ───────────────────────────────────────────────────────────────

sns.set_theme(style="whitegrid")
plt.rcParams['figure.dpi'] = 120

# Chart 1: Top changed curves – overlaid lines
top_curves = df_diff.groupby('curve')['abs_diff'].max().nlargest(6).index

fig1, ax1 = plt.subplots(figsize=(12, 6.5))
for curve in top_curves:
    sub = df_diff[df_diff['curve'] == curve].sort_values('maturity')
    ax1.plot(sub['maturity'], sub['yield_m01'], '--', alpha=0.7, label=f"{curve} M01")
    ax1.plot(sub['maturity'], sub['yield_m02'], '-', lw=2.2, label=f"{curve} M02")

ax1.set_title("Top Changed Yield Curves – M01 vs M02")
ax1.set_ylabel("Yield (%)")
ax1.set_xlabel("Maturity")
ax1.legend(bbox_to_anchor=(1.02, 1), loc='upper left')
plt.tight_layout()
fig1.savefig(IMG_DIR / "top_curves_overlaid.png", bbox_inches='tight')
plt.close(fig1)

# Chart 2: Average difference by maturity
avg_diff_by_mat = df_diff.groupby('maturity')['diff_bp'].mean().sort_index()

fig2, ax2 = plt.subplots(figsize=(10, 5.5))
colors = ['salmon' if x > 0 else 'lightgreen' for x in avg_diff_by_mat]
avg_diff_by_mat.plot(kind='bar', color=colors, ax=ax2)
ax2.axhline(0, color='gray', ls='--', lw=1.1)
ax2.set_title("Average Yield Change by Maturity (M02 – M01)")
ax2.set_ylabel("Change (basis points)")
ax2.set_xlabel("Maturity")
plt.xticks(rotation=45)
plt.tight_layout()
fig2.savefig(IMG_DIR / "avg_change_by_maturity.png", bbox_inches='tight')
plt.close(fig2)

# ───────────────────────────────────────────────────────────────
#  4. Build Markdown report
# ───────────────────────────────────────────────────────────────

md_content = f"""# {REPORT_TITLE}
**{REPORT_DATE}**

## Executive Summary

- Common curves: **{len(common)}**
- Only in M01 (disappeared): **{len(only_m01)}**  
  {', '.join(only_m01) if len(only_m01) <= 5 else ', '.join(list(only_m01)[:5]) + ', ...'}
- Only in M02 (new): **{len(only_m02)}**  
  {', '.join(only_m02) if len(only_m02) <= 5 else ', '.join(list(only_m02)[:5]) + ', ...'}

- Largest absolute move: **{top_abs.iloc[0]['diff_bp']:+.0f} bp**  
  ({top_abs.iloc[0]['curve']} – {top_abs.iloc[0]['maturity']})

- Dominant direction: **{'upward' if top_abs['diff_bp'].mean() > 0 else 'downward'}**

![Top changed curves](images/top_curves_overlaid.png)

![Average change per maturity](images/avg_change_by_maturity.png)

## Top 10 Largest Absolute Changes (bp)

```markdown
{top_abs.to_markdown(index=False)}
```

## Top 10 Largest Relative Changes (%)

```markdown
{top_rel.to_markdown(index=False)}
```

## Notes

- Yields assumed to be in decimal form (e.g. 0.0312 = 3.12%)
- Differences in bp = (M02 - M01) × 100
- Report generated automatically – adapt paths / data loading as needed

"""

with open(MD_FILE, "w", encoding="utf-8") as f:
    f.write(md_content)

print(f"Report generated:")
print(f"  Markdown:  {MD_FILE}")
print(f"  Images:    {IMG_DIR}")
print("\nTo create PDF (if pandoc is installed):")
print(f"    pandoc {MD_FILE} -o report.pdf --pdf-engine=xelatex -V geometry:margin=2.2cm")
print("\nOr open report.md in Typora / Obsidian / VS Code and export to PDF.")
```

### Quick customization points:

1. Replace `df_m01` and `df_m02` with your real data
2. Adjust maturity sorting if needed (e.g. convert to months/days numeric for better ordering)
3. Add more charts (small multiples, heatmaps, etc.)
4. Change styling / colors / fonts via seaborn/matplotlib rcParams
5. Add logo/company header by inserting HTML/Markdown at the top

Let me know which part you'd like to extend:

- reading from Excel/CSV
- maturity sorting logic
- more statistical summaries
- interactive version (plotly + HTML)
- email export / automation

Happy reporting!












