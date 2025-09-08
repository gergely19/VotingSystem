using Bunit;
using Moq;
using VotingSystem.Blazor.WebAssembly.Pages.Poll;
using VotingSystem.Blazor.WebAssembly.ViewModels;
using VotingSystem.Blazor.WebAssembly.Services;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using System;
using System.Collections.Generic;
using AutoMapper;
using Bunit.TestDoubles;

namespace VotingSystem.Tests.ComponentTests;

public class BlazorPollViewComponentsTests : IDisposable
{
    private readonly TestContext _context;
    private readonly Mock<IPollService> _pollServiceMock;

    public BlazorPollViewComponentsTests()
    {
        _context = new TestContext();
        _pollServiceMock = new Mock<IPollService>();
        _context.Services.AddSingleton(_pollServiceMock.Object);

        var mapperMock = new Mock<IMapper>();
        mapperMock.Setup(m => m.Map<PollViewModel>(It.IsAny<object>()))
                  .Returns((object src) => src as PollViewModel ?? new PollViewModel());
        _context.Services.AddSingleton<IMapper>(mapperMock.Object);

    }

    public void Dispose() => _context.Dispose();

    [Fact]
    public void PollDetails_WhenPollIsNull_ShowsLoading()
    {
        // Arrange
        var pollId = Guid.NewGuid();
        _pollServiceMock.Setup(s => s.GetPollByIdAsync(pollId)).ReturnsAsync((PollViewModel?)null!);

        // Act
        var cut = _context.RenderComponent<PollView>(parameters => parameters
            .Add(p => p.PollId, pollId));

        // Assert
        Assert.Contains("Szavazások betöltése...", cut.Markup);
    }

    [Fact]
    public void PollDetails_WhenPollLoaded_ShowsPollData()
    {
        // Arrange
        var pollId = Guid.NewGuid();
        var poll = new PollViewModel
        {
            Id = pollId,
            Question = "Teszt kérdés",
            StartDate = new DateTime(2025, 1, 1, 10, 0, 0),
            EndDate = new DateTime(2025, 1, 10, 18, 0, 0),
            Options = new List<OptionViewModel>
            {
                new OptionViewModel { Id = Guid.NewGuid(), Text = "Opció 1", VoteCount = 3 },
                new OptionViewModel { Id = Guid.NewGuid(), Text = "Opció 2", VoteCount = 5 }
            },
            UserPolls = new List<UserPollViewModel>
            {
            }
        };

        _pollServiceMock.Setup(s => s.GetPollByIdAsync(pollId)).ReturnsAsync(poll);

        // Act
        var cut = _context.RenderComponent<PollView>(parameters => parameters
            .Add(p => p.PollId, pollId));

        // Assert
        Assert.Contains(poll.Question, cut.Markup);
        Assert.Contains(poll.StartDate.ToString("yyyy-MM-dd HH:mm"), cut.Markup);
        Assert.Contains(poll.EndDate.ToString("yyyy-MM-dd HH:mm"), cut.Markup);

        foreach (var option in poll.Options)
        {
            Assert.Contains(option.Text, cut.Markup);
            Assert.Contains($"{option.VoteCount} votes", cut.Markup);
        }

        foreach (var userPoll in poll.UserPolls)
        {
            Assert.Contains(userPoll.UserId.ToString(), cut.Markup);
            if (userPoll.HasVoted)
                Assert.Contains("Szavazott", cut.Markup);
            else
                Assert.Contains("Nem szavazott", cut.Markup);
        }
    }
}
