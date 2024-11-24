# Glicko
A Unity C# implementation of the Glicko-2 player rating system sourced from [@TaylorP](https://github.com/TaylorP) in his C++ implementation [TaylorP/Glicko2](https://github.com/TaylorP/Glicko2). Glicko and Glicko-2 are alternatives to traditional Elo rating systems. Glicko was developed by Mark Glickman; the theoretical details of the system is described on [Dr. Glickman's website](http://www.glicko.net/glicko.html). Both the original Glicko and Glicko-2 rating systems are in the public domain.

# Usage
The rating system has two components: the configuration and rating objects. The values in `Glicko` described all of the configurable components and can be changed in the project settings in `Edit -> ProjectSettings/CondorHalcon/Glicko`; this includes the default rating values and system constants. Details of how to configure the rating system are described in the [Glicko-2 paper](http://www.glicko.net/glicko/glicko2.pdf).

New ratings can be created as follows:

```csharp
    // Create a new Rating instance with the default rating
    Rating rating = new Rating(Glicko.DefaultRating);

    // Create a new Rating instance with a specific rating
    Rating rating = new Rating(1500);

    // Create a new Rating instance with a specific rating and deviation
    Rating rating = new Rating(1500, 200);

    // Create a new Rating instance with a specific rating, deviation and volatility
    Rating rating = new Rating(1500, 310, 0.04)
```

In the Glicko rating system, player ratings are updated in batches. As described in the [Glicko-2 paper](http://www.glicko.net/glicko/glicko2.pdf), the ideal number of games in a rating period is 10-15. As such, the primary method for updating a rating is the `Rating.Update()` method. This arguments to the `Rating.Update()` method are a game count, an array of opponent ratings and the outcome of the games. As per the Glicko-2 paper, a victor has the score `1.0`, a draw is scored as `0.5` and a loss is `0.0`.

For example:

```csharp
    // The test player
    Rating player = new Rating(1500, 200);

    // The three opponents
    Rating opponent1 = new Rating(1400, 30);
    Rating opponent2 = new Rating(1550, 100);
    Rating opponent3 = new Rating(1700, 300);

    // Update the ratings from the 3 games
    Match[] matches = new Match[]{ new Match(opponent1, 1.0), new Match(opponent2, 0), new Match(opponent3, 0)};
    player.Update(matches);
    player.Apply();
```

A version of `Rating.Update()` that takes a single opponent and game outcome can also be used. This method is useful in situations where the ratings are updated with each game, such as an online chess environment:

```csharp
    // Player1's rating
    Rating player1 = new Rating(1500, 200);

    // Player2's rating
    Rating player2 = new Rating(1400, 30);

    // Update the ratings based on a game in which player1 beat player2
    player1.Update(new Match(player2, 1.0));
    player2.Update(new Match(player1, 0.0));

    player1.Apply();
    player2.Apply();
```

If a player does not play any games during a rating period, the Glicko-2 document recommends that their rating deviation is updated based on the volatility, as described in Step 6 of the Glicko pdf. This can be achieved using the `Rating.Decay()` function:

```csharp
    player.Decay()
```

After calling `Rating.Update()` or `Rating.Decay()`, the changes must be applied using the `Rating.Apply()` method. This is necessary because updates to multiple `Rating` instances may depend on each other and ratings should be not updated until all outcomes have been processed.

```csharp
    player.Apply();
```

# License
This implementation of the Glicko-2 rating system is distributed under [The MIT License](https://opensource.org/licenses/MIT).
