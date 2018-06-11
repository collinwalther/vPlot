from typing import Tuple
import csv


def foo(x: float, y: float, z: float) -> Tuple[float, float, float]:
    # Math function goes here
    xArrow = 1
    yArrow = 1
    zArrow = 1
    return (xArrow, yArrow, zArrow)

if __name__ == "__main__":
    with open("VectorField.csv", 'w', newline="\n") as dst:
        writer = csv.writer(dst)
        label = 0
        header = ["x", "y", "z", "xArrow", "yArrow", "zArrow", "label"]
        writer.writerow(header)
        lim = 2
        for x in range(-lim, lim + 1):
            for y in range(-lim, lim + 1):
                for z in range(-lim, lim + 1):
                    xArrow, yArrow, zArrow = foo(x, y, z)
                    writer.writerow([x, y, z, xArrow, yArrow, zArrow, label])
                    label += 1
                    
