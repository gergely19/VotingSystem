using Bunit;
using Moq;
using VotingSystem.Blazor.WebAssembly.Pages; // vagy ahol PollAdd van
using VotingSystem.Blazor.WebAssembly.Services;
using VotingSystem.Blazor.WebAssembly.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Bunit.TestDoubles;
using VotingSystem.Blazor.WebAssembly.Pages.Poll;
using Xunit.Abstractions;
using System.ComponentModel.DataAnnotations;
using AngleSharp.Dom;
using AngleSharp;

namespace VotingSystem.Tests.ComponentTests;
public class BlazorPollAddComponentsTests : IDisposable
{
    private readonly TestContext _context = new();
    private readonly Mock<IPollService> _pollServiceMock = new();
    private readonly FakeNavigationManager _navManager;
    private readonly ITestOutputHelper _output;


    public BlazorPollAddComponentsTests(ITestOutputHelper helper)
    {
        _context.Services.AddSingleton<IPollService>(_pollServiceMock.Object);
        _navManager = _context.Services.GetRequiredService<FakeNavigationManager>();
        _output = helper;
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    [Fact]
    public void PollAdd_ShouldRenderFormWithTwoOptionsInitially()
    {
        // Act
        var cut = _context.RenderComponent<PollAdd>();

        var inputs = cut.FindAll("input.form-control");
        var textInputs = inputs.Where(input =>
            input.GetAttribute("type") != "datetime-local").ToList();

        Assert.True(textInputs.Count >= 3); 
        Assert.Equal(string.Empty, textInputs[1].GetAttribute("value"));
        Assert.Equal(string.Empty, textInputs[2].GetAttribute("value")); 
    }


    [Fact]
    public void PollAdd_AddOptionButton_ShouldAddNewOptionInput()
    {
        var cut = _context.RenderComponent<PollAdd>();

        var addButton = cut.Find("button.btn.btn-success");
        addButton.Click();

        var inputs = cut.FindAll("input.form-control");
        Assert.True(inputs.Count >= 4);
    }
  
}
