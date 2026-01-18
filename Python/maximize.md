def max_products_exact(prices: list[int], total_money: int) -> int:
    """
    Returns the maximum number of products you can buy using EXACTLY total_money.
    Returns -1 if it's impossible to spend exactly that amount.
    
    Similar to unbounded knapsack / minimum coins but we maximize count instead.
    """
    if total_money == 0:
        return 0
    if not prices:
        return -1
        
    # We use +1 because we want to distinguish "impossible" from 0
    INF = float('inf')
    dp = [INF] * (total_money + 1)     # dp[m] = max number of items to make exactly m
    dp[0] = 0                           # 0 money → 0 items
    
    for price in prices:
        if price == 0: continue         # skip invalid prices
        
        for money in range(price, total_money + 1):
            if dp[money - price] != INF:
                dp[money] = max(dp[money], dp[money - price] + 1)
    
    return dp[total_money] if dp[total_money] != INF else -1


# ────────────────────────────────────────────────────────────────
# Alternative version - more memory efficient (usually enough)
# ────────────────────────────────────────────────────────────────
def max_products_exact_space_optimized(prices: list[int], total: int) -> int:
    dp = [-1] * (total + 1)
    dp[0] = 0
    
    for price in prices:
        if price <= 0:
            continue
        for m in range(price, total + 1):
            prev = dp[m - price]
            if prev != -1:
                dp[m] = max(dp[m], prev + 1)
    
    return dp[total]


# Quick test / example usage
if __name__ == "__main__":
    prices = [7, 13, 5, 3, 8]
    money = 31
    
    result = max_products_exact(prices, money)
    if result == -1:
        print(f"Cannot spend exactly {money}€ with these prices")
    else:
        print(f"Maximum products with exactly {money}€ → {result} pieces")
