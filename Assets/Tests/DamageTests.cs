using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

[TestFixture]
public class DamageTests 
{
    [UnityTest]
    public IEnumerator TakeDamage_ReduceHealth()
    {
        // Arrange
        DamageableEntity damageable = new GameObject().AddComponent<DamageableEntity>();
        float initialHealth = damageable.MaxHealth;
        float damageTaken = 10;

        // Act
        damageable.TakeDamage(damageTaken);
        yield return null;
        
        // Assert
        Assert.AreEqual(initialHealth - damageTaken, damageable.Health);
        
    }

    [UnityTest]
    public IEnumerator Die_SetDeadFlagAndInvokeDeathEvent()
    {
        // Arrange
        DamageableEntity damageable = new GameObject().AddComponent<DamageableEntity>();
        bool deathEventTriggered = false;
        IDamageable.OnDeath += () => deathEventTriggered = true;

        // Act
        damageable.TakeDamage(damageable.MaxHealth);
        yield return null;

        // Assert
        Assert.IsTrue(damageable.Dead);
        Assert.IsTrue(deathEventTriggered);
    }

    [UnityTest]
    public IEnumerator TurretKillsAnEntity()
    {
        // Arrange
        SceneManager.LoadScene("DamageTestScene");
        yield return null;
        DamageableEntity damageable = GameObject.Find("Drone").GetComponent<DamageableEntity>();

        // Act
        bool deathEventTriggered = false;
        IDamageable.OnDeath += () => deathEventTriggered = true;
        yield return new WaitUntil(() => deathEventTriggered);

        // Assert
        Assert.IsTrue(deathEventTriggered);
    }
}

public class DamageableEntity : IDamageable 
{
}