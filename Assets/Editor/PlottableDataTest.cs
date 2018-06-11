using NUnit.Framework;
using UnityEngine;

public class PlottableDataTest
{
    private static float[][] dummyData = {
        new float[] { 1, 2, 3, 4, 5 },
        new float[] { 6, 7, 8, 9, 10 },
        new float[] { 15, 14, 13, 12, 11 },
        new float[] { -4, -2, -5, -1, 0 }
    };

    [Test]
    public void GetParticles_WithEmptyData_DoesntThrowException()
    {
        // Generate the empty data.
        float[][] emptyData = new float[0][];
        PlottableData data = new PlottableData(emptyData);

        // Make sure that getting the particles doesn't throw an exception.
        ParticleSystem.Particle[] particles = data.GetParticles();
        Assert.AreEqual(particles.Length, 0);
    }

    [Test]
    public void GetParticles_WithRealDataAndNoPropertiesSet_DoesntThrowException()
    {
        PlottableData data = new PlottableData(dummyData);

        // Get the particles.
        ParticleSystem.Particle[] particles = data.GetParticles();
        Assert.AreEqual(dummyData.Length, particles.Length);
    }

    [Test]
    public void GetParticles_WithRealDataAndPropertiesSet_DoesntThrowException()
    {
        PlottableData data = new PlottableData(dummyData);

        // Set custom indices for the data.
        int xSpatialFieldIndex = 3;
        int ySpatialFieldIndex = 2;
        int zSpatialFieldIndex = 1;
        data.XSpatialFieldIndex = xSpatialFieldIndex;
        data.YSpatialFieldIndex = ySpatialFieldIndex;
        data.ZSpatialFieldIndex = zSpatialFieldIndex;

        // Get the particles.
        ParticleSystem.Particle[] particles = data.GetParticles();
        Assert.AreEqual(dummyData.Length, particles.Length);

        // Ensure that the particles are properly generated.
        for (int i = 0; i < particles.Length; i++)
        {
            Assert.AreEqual(dummyData[i][xSpatialFieldIndex], particles[i].position.x);
            Assert.AreEqual(dummyData[i][ySpatialFieldIndex], particles[i].position.y);
            Assert.AreEqual(dummyData[i][zSpatialFieldIndex], particles[i].position.z);
        }
    }
}
