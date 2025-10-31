import random
import math

def monte_carlo_pi(num_samples):
    """
    Estimate the value of π using Monte Carlo simulation.

    Args:
        num_samples (int): Number of random points to generate.

    Returns:
        float: Estimated value of π.
    """
    inside_circle = 0

    for _ in range(num_samples):
        # Generate random (x, y) point in the range [0, 1]
        x = random.uniform(0, 1)
        y = random.uniform(0, 1)

        # Check if the point lies inside the unit circle
        if math.sqrt(x**2 + y**2) <= 1:
            inside_circle += 1

    # π is approximately 4 times the ratio of points inside the circle to total points
    return 4 * (inside_circle / num_samples)

if __name__ == "__main__":
    # Number of random samples
    num_samples = 1_000_000_000

    # Estimate π
    estimated_pi = monte_carlo_pi(num_samples)

    # Display the result
    print(f"Estimated value of π using {num_samples} samples: {estimated_pi}")
