using Moq;
using TicketNotificationApi.Application.Abstractions;
using TicketNotificationApi.Application.Notifications;
using TicketNotificationApi.Application.Tests.Fakes;
using TicketNotificationApi.Domain.Entities;
using TicketNotificationApi.Domain.Enums;

namespace TicketNotificationApi.Application.Tests;

public class NotificationOrchestratorTests
{
    private readonly FakeTicketRepository _ticketRepo;
    private readonly FakeNotificationRepository _notificationRepo;
    private readonly Mock<INotificationSender> _emailMock;
    private readonly Mock<INotificationSender> _smsMock;
    private readonly Mock<INotificationSender> _pushMock;
    private readonly NotificationOrchestrator _orchestrator;

    public NotificationOrchestratorTests()
    {
        _ticketRepo = new FakeTicketRepository();
        _notificationRepo = new FakeNotificationRepository();

        _emailMock = CreateSenderMock(NotificationChannel.Email);
        _smsMock = CreateSenderMock(NotificationChannel.SMS);
        _pushMock = CreateSenderMock(NotificationChannel.Push);

        var senders = new List<INotificationSender> { _emailMock.Object, _smsMock.Object, _pushMock.Object };

        _orchestrator = new NotificationOrchestrator(_ticketRepo, _notificationRepo, senders);
    }

    private static Mock<INotificationSender> CreateSenderMock(NotificationChannel channel)
    {
        var mock = new Mock<INotificationSender>();
        mock.SetupGet(m => m.Channel).Returns(channel);
        return mock;
    }

    [Fact]
    public async Task CreateNotificationsForTicketAsync_CreatesThreePendingNotifications()
    {
        // Arrange
        var ticket = new Ticket { Id = Guid.NewGuid(), Title = "Test Ticket" };

        // Act
        var notifications = (await _orchestrator.CreateNotificationsForTicketAsync(ticket)).ToList();

        // Assert
        Assert.Equal(3, notifications.Count);
        Assert.All(notifications, n => Assert.Equal(NotificationStatus.Pending, n.Status));
        Assert.Contains(notifications, n => n.Channel == NotificationChannel.Email);
        Assert.Contains(notifications, n => n.Channel == NotificationChannel.SMS);
        Assert.Contains(notifications, n => n.Channel == NotificationChannel.Push);
    }

    [Fact]
    public async Task SendPendingNotificationsAsync_AllSucceed_UpdatesAllToSent()
    {
        // Arrange
        var ticket = new Ticket { Id = Guid.NewGuid(), Title = "Test" };
        await _ticketRepo.AddAsync(ticket);
        await _orchestrator.CreateNotificationsForTicketAsync(ticket);

        _emailMock.SetupSendToReturn(NotificationSendResult.Ok());
        _smsMock.SetupSendToReturn(NotificationSendResult.Ok());
        _pushMock.SetupSendToReturn(NotificationSendResult.Ok());

        // Act
        var results = (await _orchestrator.SendPendingNotificationsAsync(ticket.Id)).ToList();

        // Assert
        Assert.Equal(3, results.Count);
        Assert.All(results, n => Assert.Equal(NotificationStatus.Sent, n.Status));
        Assert.All(results, n => Assert.Null(n.LastError));
    }

    [Fact]
    public async Task SendPendingNotificationsAsync_RepeatedFailure_StopsAfterThreeAttempts()
    {
        // Arrange
        var ticket = new Ticket { Id = Guid.NewGuid(), Title = "Test" };
        await _ticketRepo.AddAsync(ticket);
        await _orchestrator.CreateNotificationsForTicketAsync(ticket);

        _emailMock.SetupSendToReturn(NotificationSendResult.Fail("Simulated error"));
        _smsMock.SetupSendToReturn(NotificationSendResult.Fail("Simulated error"));
        _pushMock.SetupSendToReturn(NotificationSendResult.Fail("Simulated error"));

        // Act
        for (int i = 0; i < 4; i++)
        {
            await _orchestrator.SendPendingNotificationsAsync(ticket.Id);
        }

        // Assert
        var results = await _notificationRepo.GetByTicketIdAsync(ticket.Id);
        Assert.All(results, n => Assert.Equal(3, n.Attempts));

        _emailMock.Verify(m => m.SendAsync(ticket, It.IsAny<Notification>(), It.IsAny<CancellationToken>()), Times.Exactly(3));
        _smsMock.Verify(m => m.SendAsync(ticket, It.IsAny<Notification>(), It.IsAny<CancellationToken>()), Times.Exactly(3));
        _pushMock.Verify(m => m.SendAsync(ticket, It.IsAny<Notification>(), It.IsAny<CancellationToken>()), Times.Exactly(3));
    }

    [Fact]
    public async Task SendPendingNotificationsAsync_CalledTwice_IsIdempotent_NoDuplicateNotifications()
    {
        // Arrange
        var ticket = new Ticket { Id = Guid.NewGuid(), Title = "Test" };
        await _ticketRepo.AddAsync(ticket);
        await _orchestrator.CreateNotificationsForTicketAsync(ticket);

        _emailMock.SetupSendToReturn(NotificationSendResult.Ok());
        _smsMock.SetupSendToReturn(NotificationSendResult.Ok());
        _pushMock.SetupSendToReturn(NotificationSendResult.Ok());

        // Act
        await _orchestrator.SendPendingNotificationsAsync(ticket.Id);
        await _orchestrator.SendPendingNotificationsAsync(ticket.Id);

        // Assert
        var results = (await _notificationRepo.GetByTicketIdAsync(ticket.Id)).ToList();

        Assert.Equal(3, results.Count);
        Assert.All(results, n => Assert.Equal(NotificationStatus.Sent, n.Status));

        _emailMock.Verify(m => m.SendAsync(ticket, It.IsAny<Notification>(), It.IsAny<CancellationToken>()), Times.Once);
        _smsMock.Verify(m => m.SendAsync(ticket, It.IsAny<Notification>(), It.IsAny<CancellationToken>()), Times.Once);
        _pushMock.Verify(m => m.SendAsync(ticket, It.IsAny<Notification>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SendPendingNotificationsAsync_OnFailure_StoresErrorMessageInLastError()
    {
        // Arrange
        var ticket = new Ticket { Id = Guid.NewGuid(), Title = "Test" };
        await _ticketRepo.AddAsync(ticket);
        await _orchestrator.CreateNotificationsForTicketAsync(ticket);

        var expectedError = "SMTP connection timed out.";
        _emailMock.SetupSendToReturn(NotificationSendResult.Fail(expectedError));
        _smsMock.SetupSendToReturn(NotificationSendResult.Ok());
        _pushMock.SetupSendToReturn(NotificationSendResult.Ok());

        // Act
        var results = await _orchestrator.SendPendingNotificationsAsync(ticket.Id);

        // Assert
        var emailNotification = results.Single(n => n.Channel == NotificationChannel.Email);
        Assert.Equal(NotificationStatus.Failed, emailNotification.Status);
        Assert.Equal(expectedError, emailNotification.LastError);
        Assert.Equal(1, emailNotification.Attempts);
    }

    [Fact]
    public async Task SendPendingNotificationsAsync_UnknownTicketId_ThrowsKeyNotFoundException()
    {
        // Arrange
        var missingTicketId = Guid.NewGuid();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _orchestrator.SendPendingNotificationsAsync(missingTicketId));
    }
}

internal static class MockExtensions
{
    public static void SetupSendToReturn(this Mock<INotificationSender> mock, NotificationSendResult result)
    {
        mock.Setup(m => m.SendAsync(It.IsAny<Ticket>(), It.IsAny<Notification>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);
    }
}
