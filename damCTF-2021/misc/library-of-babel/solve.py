import anvil
import re

region = anvil.Region.from_file("./region/r.-1.0.mca")

chunk = anvil.Chunk.from_region(region, 21, 7)

for x in range(31):
    for z in range(31):
        try:
            chunk = anvil.Chunk.from_region(region, x, z)
            for te in chunk.tile_entities:
                if str(te["id"]) == "minecraft:lectern":
                    try:
                        pages = te["Book"]["tag"]["pages"]
                    except KeyError as e:
                        print(e)
                        print(te.id)
                        print(te)
                    for page in pages:
                        if "dam{" in str(page):
                            #print(page)
                            print(re.findall(r"dam{.*?}", str(page))[0])
                        else:
                            continue
                else:
                    continue
        except anvil.errors.ChunkNotFound as e:
            continue