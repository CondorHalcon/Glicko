using NUnit.Framework;

namespace CondorHalcon.Glicko.Tests
{
    public class GlickoTests
    {
        [Test]
        public void Glicko1()
        {

            // The test player. Example taken from
            // http://www.glicko.net/glicko/glicko2.pdf
            Rating player = new Rating(1500, 200);

            // The three opponents
            Rating player1 = new Rating(1400, 30);
            Rating player2 = new Rating(1550, 100);
            Rating player3 = new Rating(1700, 300);

            // Simulate 3 games and update the ratings

            Match[] players = { new Match(player1, 1), new Match(player2, 0), new Match(player3, 0) };
            player.Update(players);
            player.Apply();

            Assert.AreEqual(player.Rating1, 1464.06, .01);
            Assert.AreEqual(player.Deviation1, 151.52, .01);
        }

        [Test]
        public void Glicko2()
        {
            // The test player. Example taken from
            // http://www.glicko.net/glicko/glicko2.pdf
            Rating player = new Rating(1500, 200, 0.06);

            // The three opponents
            Rating player1 = new Rating(1400, 30);
            Rating player2 = new Rating(1550, 100);
            Rating player3 = new Rating(1700, 300);

            // Simulate 3 games and update the ratings

            Match[] players = { new Match(player1, 1), new Match(player2, 0), new Match(player3, 0) };
            player.Update(players);
            player.Apply();

            Assert.AreEqual(player.Rating2, -0.20694096667525494, .01);
            Assert.AreEqual(player.Deviation2, 0.87219918813073427, .01);
            Assert.AreEqual(player.Volatility2, 0.05999, .01);
        }
    }
}
