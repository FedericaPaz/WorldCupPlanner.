using Domain.Entities;
using DomainTests.Stubs;

namespace DomainTests;

[TestClass]
public class NotificationTests
{
    [TestMethod]
    public void CreateNotification_WithValidData_ShouldCreateUnreadNotification()
    {
        var createdAt = new DateTime(2026, 6, 1, 14, 30, 0);

        var notification = new Notification(
            "periodista@test.com",
            "Se cargó un nuevo resultado",
            createdAt);

        Assert.AreEqual("periodista@test.com", notification.UserEmail);
        Assert.AreEqual("periodista@test.com", notification.RecipientEmail);
        Assert.AreEqual("Se cargó un nuevo resultado", notification.Message);
        Assert.AreEqual(createdAt, notification.CreatedAt);
        Assert.AreEqual(createdAt, notification.Timestamp);
        Assert.IsFalse(notification.IsRead);
    }

    [TestMethod]
    public void CreatingNotification_WithValidData_ShouldStartAsUnread()
    {
        var notification = new Notification(
            "journalist@test.com",
            "Resultado cargado.");

        Assert.IsFalse(notification.IsRead);
    }

    [TestMethod]
    public void CreatingNotification_WithValidData_ShouldStoreRecipientEmail()
    {
        var notification = new Notification(
            "journalist@test.com",
            "Resultado cargado.");

        Assert.AreEqual("journalist@test.com", notification.RecipientEmail);
    }

    [TestMethod]
    public void CreateNotification_WithValidData_ShouldStoreMessage()
    {
        var notification = new Notification(
            "journalist@test.com",
            "Mensaje");

        Assert.AreEqual("Mensaje", notification.Message);
    }

    [TestMethod]
    public void CreateNotification_WithValidData_ShouldSetTimestampToCurrentTime()
    {
        var now = new DateTime(2026, 6, 1, 10, 30, 0);
        var clock = new DateTimeMatchStub(now);

        var notification = new Notification(
            "journalist@test.com",
            "Resultado cargado.",
            clock);

        Assert.AreEqual(now, notification.Timestamp);
    }

    [TestMethod]
    public void UnreadNotification_MarkingAsRead_SetsIsReadToTrue()
    {
        var notification = new Notification(
            "journalist@test.com",
            "Resultado cargado.");

        notification.MarkAsRead();

        Assert.IsTrue(notification.IsRead);
    }

    [TestMethod]
    public void MarkAsRead_WhenNotificationIsUnread_ShouldSetReadAt()
    {
        var notification = new Notification(
            "periodista@test.com",
            "Se cargó un nuevo resultado",
            new DateTime(2026, 6, 1, 14, 30, 0));

        var readAt = new DateTime(2026, 6, 1, 15, 0, 0);

        notification.MarkAsRead(readAt);

        Assert.AreEqual(readAt, notification.ReadAt);
    }

    [TestMethod]
    public void UnreadNotification_MarkingAsRead_ShouldSetReadAt()
    {
        var readAt = new DateTime(2026, 6, 1, 12, 0, 0);
        var clock = new DateTimeMatchStub(readAt);

        var notification = new Notification(
            "journalist@test.com",
            "Resultado cargado.");

        notification.MarkAsRead(clock);

        Assert.AreEqual(readAt, notification.ReadAt);
    }

    [TestMethod]
    public void EmptyRecipientEmail_CreatingNotification_ThrowsArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            new Notification("", "Resultado cargado."));
    }

    [TestMethod]
    public void EmptyMessage_CreatingNotification_ThrowsArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            new Notification("journalist@test.com", ""));
    }

    [TestMethod]
    public void CreatingNotification_WithValidData_ShouldGenerateId()
    {
        var notification = new Notification(
            "journalist@test.com",
            "Resultado cargado.");

        Assert.AreNotEqual(Guid.Empty, notification.Id);
    }
}