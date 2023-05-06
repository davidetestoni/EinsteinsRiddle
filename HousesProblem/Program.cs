using System.Diagnostics;

IEnumerable<House> EnumerateOptions(params House[] others)
{
    foreach (var color in Enum.GetValues<Color>())
    {
        if (others.Any(h => h.Color == color))
        {
            continue;
        }

        foreach (var nationality in Enum.GetValues<Nationality>())
        {
            if (others.Any(h => h.Nationality == nationality))
            {
                continue;
            }

            foreach (var beverage in Enum.GetValues<Beverage>())
            {
                if (others.Any(h => h.Beverage == beverage))
                {
                    continue;
                }

                foreach (var cigar in Enum.GetValues<Cigar>())
                {
                    if (others.Any(h => h.Cigar == cigar))
                    {
                        continue;
                    }

                    foreach (var pet in Enum.GetValues<Pet>())
                    {
                        if (others.Any(h => h.Pet == pet))
                        {
                            continue;
                        }

                        yield return new(color, nationality, beverage, cigar, pet);
                    }
                }
            }
        }
    }
}

bool NeighborHas(House[] houses, int i, Func<House, bool> func)
{
    House? leftNeighbor = null;
    House? rightNeighbor = null;

    if (i != 0)
    {
        leftNeighbor = houses[i - 1];
    }

    if (i != houses.Length - 1)
    {
        rightNeighbor = houses[i + 1];
    }

    return (leftNeighbor != null && func.Invoke(leftNeighbor)) || (rightNeighbor != null && func.Invoke(rightNeighbor));
}

bool IsValid(House house)
{
    // The Brit lives in the red house
    if (house.Color == Color.Red && house.Nationality != Nationality.British)
    {
        return false;
    }

    // The Swede keeps dogs as pets
    if (house.Nationality == Nationality.Swedish && house.Pet != Pet.Dogs)
    {
        return false;
    }

    // The Dane drinks tea
    if (house.Nationality == Nationality.Danish && house.Beverage != Beverage.Tea)
    {
        return false;
    }

    // The person who smokes Pall Malls rears birds
    if (house.Cigar == Cigar.PallMalls && house.Pet != Pet.Birds)
    {
        return false;
    }

    // The owner of the yellow house smokes Dunhill
    if (house.Color == Color.Yellow && house.Cigar != Cigar.Dunhill)
    {
        return false;
    }

    // The green house’s owner drinks coffee
    if (house.Color == Color.Green && house.Beverage != Beverage.Coffee)
    {
        return false;
    }

    // The owner who smokes BlueMaster drinks beer
    if (house.Cigar == Cigar.BlueMaster && house.Beverage != Beverage.Beer)
    {
        return false;
    }

    // The German smokes Princes
    if (house.Nationality == Nationality.German && house.Cigar != Cigar.Princes)
    {
        return false;
    }

    return true;
}

bool AreValid(House[] houses)
{
    // The man living in the center house drinks milk
    if (houses[2].Beverage != Beverage.Milk)
    {
        return false;
    }

    // The Norwegian lives in the first (leftmost) house
    if (houses[0].Nationality != Nationality.Norwegian)
    {
        return false;
    }

    for (int i = 0; i < houses.Length; i++)
    {
        var house = houses[i];

        // The green house is on the left of the white house
        if (house.Color == Color.Green && (i == houses.Length - 1 || houses[i + 1].Color != Color.White))
        {
            return false;
        }

        // The man who smokes Blends lives next to the one who keeps cats
        if (house.Cigar == Cigar.Blends && !NeighborHas(houses, i, h => h.Pet == Pet.Cats))
        {
            return false;
        }

        // The man who keeps horses lives next to the man who smokes Dunhill
        if (house.Pet == Pet.Horses && !NeighborHas(houses, i, h => h.Cigar == Cigar.Dunhill))
        {
            return false;
        }

        // The Norwegian lives next to the blue house
        if (house.Nationality == Nationality.Norwegian && !NeighborHas(houses, i, h => h.Color == Color.Blue))
        {
            return false;
        }

        // The man who smokes Blends has a neighbor who drinks water
        if (house.Cigar == Cigar.Blends && !NeighborHas(houses, i, h => h.Beverage == Beverage.Water))
        {
            return false;
        }
    }

    return true;
}

long done = 0;
var sw = new Stopwatch();
sw.Start();

foreach (var house1 in EnumerateOptions())
{
    if (!IsValid(house1))
    {
        continue;
    }

    foreach (var house2 in EnumerateOptions(house1))
    {
        if (!IsValid(house2))
        {
            continue;
        }

        foreach (var house3 in EnumerateOptions(house1, house2))
        {
            if (!IsValid(house3))
            {
                continue;
            }

            foreach (var house4 in EnumerateOptions(house1, house2, house3))
            {
                if (!IsValid(house4))
                {
                    continue;
                }

                foreach (var house5 in EnumerateOptions(house1, house2, house3, house4))
                {
                    if (!IsValid(house5))
                    {
                        continue;
                    }

                    done++;

                    Console.Title = $"Done: {done} - {house1.GetCode()} | {house2.GetCode()} | {house3.GetCode()} | {house4.GetCode()} | {house5.GetCode()}";

                    var houses = new House[] { house1, house2, house3, house4, house5 };
                    if (AreValid(houses))
                    {
                        sw.Stop();
                        Console.WriteLine($"Solution found! Elapsed: {sw.Elapsed}");

                        for (int i = 0; i < houses.Length; i++)
                        {
                            var house = houses[i];
                            Console.WriteLine($"{i} - {house}");
                        }

                        Console.ReadLine();
                    }
                }
            }
        }
    }
}

enum Color
{
    Blue,
    Green,
    Yellow,
    Red,
    White
}

enum Nationality
{
    British,
    Swedish,
    Danish,
    Norwegian,
    German
}

enum Beverage
{
    Tea,
    Coffee,
    Milk,
    Beer,
    Water
}

enum Cigar
{
    PallMalls,
    Dunhill,
    BlueMaster,
    Princes,
    Blends
}

enum Pet
{
    Dogs,
    Cats,
    Birds,
    Fish,
    Horses
}

record House(Color Color, Nationality Nationality, Beverage Beverage, Cigar Cigar, Pet Pet)
{
    public string GetCode() => $"{(int)Color}{(int)Nationality}{(int)Beverage}{(int)Cigar}{(int)Pet}";
}
