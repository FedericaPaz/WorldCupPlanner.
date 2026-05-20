using Domain.Entities;

namespace DomainTests;

[TestClass]
public class NotificationTests
{
    [TestMethod]
    public void CreatingNotification_WithValidData_ShouldStartAsUnread()
    {
        var notification = new Notification("journalist@test.com", "Resultado cargado.");
        Assert.IsFalse(notification.IsRead);
    }
    
    [TestMethod]
    public void CreatingNotification_WithValidData_ShouldStoreRecipientEmail()
    {
        var notification = new Notification("journalist@test.com", "Resultado cargado."
        );
        Assert.AreEqual("journalist@test.com", notification.RecipientEmail);
    }
    
    [TestMethod]
    public void CreateNotification_WithValidData_ShouldStoreMessage()
    {
        var notification = new Notification("journalist@test.com", "Mensaje");
        Assert.AreEqual("Mensaje", notification.Message);
    }
}