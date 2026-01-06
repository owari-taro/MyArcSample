import struct

def read_shapefile_extent(shp_path):
    with open(shp_path, "rb") as f:
        # extent は byte 36 から double 4つ
        f.seek(36)
        data = f.read(8 * 4)

        xmin, ymin, xmax, ymax = struct.unpack("<4d", data)

    return xmin, ymin, xmax, ymax


if __name__ == "__main__":
    shp = "example.shp"
    extent = read_shapefile_extent(shp)

    print("Extent:")
    print(f"  xmin: {extent[0]}")
    print(f"  ymin: {extent[1]}")
    print(f"  xmax: {extent[2]}")
    print(f"  ymax: {extent[3]}")