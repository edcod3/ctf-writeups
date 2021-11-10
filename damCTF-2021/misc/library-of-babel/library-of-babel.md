# Writeup: library-of-babel

## Challenge Description

> Legend has it there is a secret somewhere in this library. Unfortunately, all of our Babel Fishes have gotten lost and the books are full of junk.
> 
> _Note: You do not need a copy of Minecraft to play this challenge._ \
> The flag is in standard flag format.

## Solution 

### 1. Analyzing the files

We are given a folder which ressembles the contents of a minecraft world file structure. 
Even though the description states we don't need a copy of minecraft to solve the challenge,
I thought having a look at it in Minecraft would give us a bigger picture & help us solve the
challenge.

_Note: This is how I solved the challenge during the CTF. Opening the world in minecraft is purely optional & only makes finding the location of the book contents easier._

### 2. Inspecting the minecraft world

Copying the folder into the minecraft saves directory (in my case: `%USERPROFILE%\AppData\Roaming\.minecraft\saves`) and launching the game will load the world into the game so we can join it.

Once the world is loaded, we can see a bunch of books on "bookstands" or __lecterns__. If we open one of these books we can see a bunch of random characters.
![Random characters](https://raw.githubusercontent.com/edcod3/ctf-writeups/master/damCTF-2021/misc/library-of-babel/random_book_text.jpg "Random characters")

We can assume that the flag is hidden in one of the many books in the library & that we have to programmatically find the exact book/page with the flag in it. 


### 3. NBTExplorer & analyzing chunk data

So how can we find the book which has the flag in it? \
Searching for an answer on google, I came across this [reddit post](https://www.reddit.com/r/Minecraft/comments/38ahc2/looking_through_world_data_trying_to_find_a/).

A user suggest using the tool [NBTExplorer](https://github.com/jaquadro/NBTExplorer), which can parse `.mca` files. \
These are the files that are most interesting to us, because they contain the map/chunk data, for example book contents. More information about the file format can be found [here](https://minecraft.fandom.com/wiki/Anvil_file_format).

We can start with the biggest file (`r.-1.0.mca`) as the chances of the flag being in this file is the biggest, because it contains the most data/books (maybe even all of them). 

Opening the file in `NBTExplorer` gives us the following output:
![NBTExplorer output](https://raw.githubusercontent.com/edcod3/ctf-writeups/master/damCTF-2021/misc/library-of-babel/nbtexplorer_output.jpg "NBTExplorer output")

We can see that the file is split into chunks, but which chunks have books inside them and how can we parse the page contents? \
Instead of manually checking each chunk in NBTExplorer until we find a book (which is probably what I would have done if I hadn't had a copy of minecraft), we can just check which chunk contains a book inside of Minecraft itself!

Go back to Minecraft & press `F3` to open the debug screen. Then walk to a book/lectern & read the chunk location from the debug screen.

![Minecraft chunk location](https://raw.githubusercontent.com/edcod3/ctf-writeups/master/damCTF-2021/misc/library-of-babel/chunk_location.jpg "Minecraft chunk location")

Now we locate the exact chunk in the NBTExplorer & find the book contents.

![NBTExplorer chunk data](https://raw.githubusercontent.com/edcod3/ctf-writeups/master/damCTF-2021/misc/library-of-babel/nbtexplorer_chunk_data.jpg "NBTExplorer chunk data").

We can see that the page contents are located in `TileEntities->Book->tag->pages`.

So now that we know where to find the contents of a book, let's create a script that goes through every book in every chunk & checks if the flag is in one of the page texts.


### 4. Flag extraction

To automate the text extraction, I used the [anvil-parser](https://pypi.org/project/anvil-parser/) package.

Then I wrote a python script that iterates over every chunk (NBTExplorer shows us that the first chunk is _Chunk[0, 0]_ and the last chunk is _Chunk[31, 31]_), extracts the page contents & check if the substring `dam{` is present in the text. 

```python
import anvil
import re

region = anvil.Region.from_file("./region/r.-1.0.mca")

for x in range(31):
    for z in range(31):
        try:
            chunk = anvil.Chunk.from_region(region, x, z)
            for te in chunk.tile_entities:
                if str(te["id"]) == "minecraft:lectern":
                    pages = te["Book"]["tag"]["pages"]
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
```

Running the python script gives us the flag.

## Flag: dam{b@B3l5-b@bBL3}