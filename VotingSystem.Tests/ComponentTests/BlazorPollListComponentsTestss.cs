using Bunit;
using Moq;
using VotingSystem.Blazor.WebAssembly.Pages;
using VotingSystem.Blazor.WebAssembly.ViewModels;
using VotingSystem.Blazor.WebAssembly.Services;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bunit.TestDoubles;
using Voting.DataAccess.Models;
using VotingSystem.Blazor.WebAssembly.Pages.Poll;

namespace VotingSystem.Tests.ComponentTests;

public class BlazorPollListComponentsTests : IDisposable
{
    private readonly TestContext _context;
    private readonly Mock<IPollService> _pollServiceMock;
    private readonly FakeNavigationManager _navManager;

    public BlazorPollListComponentsTests()
    {
        _context = new TestContext();
        _pollServiceMock = new Mock<IPollService>();
        _context.Services.AddSingleton(_pollServiceMock.Object);
        _navManager = _context.Services.GetRequiredService<FakeNavigationManager>();
    }

    public void Dispose() => _context.Dispose();

    [Fact]
    public void PollsPage_WhenPollsIsNull_ShowsLoading()
    {
        // Arrange  
        _ = _pollServiceMock.Setup(static x => x.GetPollsAsync()).ReturnsAsync((List<PollViewModel>?)null!);

        // Act  
        var cut = _context.RenderComponent<PollList>();

        // Assert  
        Assert.Contains("Betöltés...", cut.Markup);
    }

    [Fact]
    public void PollsPage_WhenPollsEmpty_ShowsNoPollFound()
    {
        // Arrange
        _pollServiceMock.Setup(x => x.GetPollsAsync()).ReturnsAsync(new List<PollViewModel>());

        // Act
        var cut = _context.RenderComponent<PollList>();

        // Assert
        Assert.Contains("Nem található szavazás.", cut.Markup);
    }

    [Fact]
    public void PollsPage_WhenPollsPresent_ShowsPollCards()
    {
        // Arrange
        var polls = new List<PollViewModel>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Question = "Teszt kérdés 1",
                StartDate = new DateTime(2025, 1, 1),
                EndDate = new DateTime(2025, 1, 10)
            },
            new()
            {
                Id = Guid.NewGuid(),
                Question = "Teszt kérdés 2",
                StartDate = new DateTime(2025, 2, 1),
                EndDate = new DateTime(2025, 2, 10)
            }
        };

        _pollServiceMock.Setup(x => x.GetPollsAsync()).ReturnsAsync(polls);

        // Act
        var cut = _context.RenderComponent<PollList>();

        // Assert
        foreach (var poll in polls)
        {
            Assert.Contains(poll.Question, cut.Markup);
            Assert.Contains(poll.StartDate.ToString("yyyy-MM-dd"), cut.Markup);
            Assert.Contains(poll.EndDate.ToString("yyyy-MM-dd"), cut.Markup);
        }

    }

    [Fact]
    public void PollsPage_WhenPollClicked_NavigatesToPollDetails()
    {
        // Arrange
        var poll = new PollViewModel
        {
            Id = Guid.NewGuid(),
            Question = "Szavazás kérdés",
            StartDate = DateTime.Now,
            EndDate = DateTime.Now.AddDays(1)
        };

        _pollServiceMock.Setup(x => x.GetPollsAsync()).ReturnsAsync(new List<PollViewModel> { poll });

        // Act
        var cut = _context.RenderComponent<PollList>();
        var card = cut.Find(".card");
        card.Click();

        // Assert
        Assert.Contains($"/polls/{poll.Id}", _navManager.Uri);
    }
}
