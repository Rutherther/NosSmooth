//
//  ContractTests.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using NosSmooth.Core.Contracts;
using Remora.Results;
using Shouldly;
using Xunit;

namespace NosSmooth.Core.Tests.Contracts;

/// <summary>
/// Tests basics of contract system.
/// </summary>
public class ContractTests
{
    /// <summary>
    /// Tests that the contract is executed.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Fact]
    public async Task Test_GetsExecuted()
    {
        var contractor = new Contractor();
        var mock = new MockClass();

        var contract = new ContractBuilder<long, DefaultStates, NoErrors>(contractor, DefaultStates.None)
            .SetMoveAction(DefaultStates.None, mock.Setup, DefaultStates.Requested)
            .SetMoveFilter<ContractData<long>>(DefaultStates.Requested, DefaultStates.ResponseObtained)
            .SetFillData<ContractData<long>>(DefaultStates.ResponseObtained, d => d.Data)
            .Build();

        contract.CurrentState.ShouldBe(DefaultStates.None);
        await contract.OnlyExecuteAsync();
        contract.CurrentState.ShouldBe(DefaultStates.Requested);
        mock.Executed.ShouldBeTrue();
        mock.ExecutedTimes.ShouldBe(1);
    }

    /// <summary>
    /// Tests that the contract response is obtained.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Fact]
    public async Task Test_ResponseObtained()
    {
        var contractor = new Contractor();
        var mock = new MockClass();

        var contract = new ContractBuilder<long, DefaultStates, NoErrors>(contractor, DefaultStates.None)
            .SetMoveAction(DefaultStates.None, mock.Setup, DefaultStates.Requested)
            .SetMoveFilter<ContractData<long>>(DefaultStates.Requested, DefaultStates.ResponseObtained)
            .SetFillData<ContractData<long>>(DefaultStates.ResponseObtained, d => d.Data)
            .Build();

        await contract.OnlyExecuteAsync();
        contract.Register();
        await contractor.Update(new ContractData<long>(5));
        contract.CurrentState.ShouldBe(DefaultStates.ResponseObtained);
        contract.Data.ShouldBe(5);

        await contractor.Update(new ContractData<long>(10));
        contract.Data.ShouldBe(5);
        contract.Unregister();

        mock.ExecutedTimes.ShouldBe(1);
    }

    /// <summary>
    /// Tests that the contract response is obtained.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Fact]
    public async Task Test_WaitFor()
    {
        var contractor = new Contractor();
        var mock = new MockClass();

        var contract = new ContractBuilder<long, DefaultStates, NoErrors>(contractor, DefaultStates.None)
            .SetMoveAction(DefaultStates.None, mock.Setup, DefaultStates.Requested)
            .SetMoveFilter<ContractData<long>>(DefaultStates.Requested, DefaultStates.ResponseObtained)
            .SetFillData<ContractData<long>>(DefaultStates.ResponseObtained, d => d.Data)
            .Build();

        Task.Run
        (
            async () =>
            {
                await Task.Delay(500);
                await contractor.Update(new ContractData<long>(15));
            }
        );

        var result = await contract.WaitForAsync(DefaultStates.ResponseObtained);
        result.IsSuccess.ShouldBeTrue();
        result.Entity.ShouldBe(15);
        contract.IsRegistered.ShouldBeFalse(); // trust the contract for now.
        contract.CurrentState.ShouldBe(DefaultStates.ResponseObtained);
        mock.ExecutedTimes.ShouldBe(1);
    }

    /// <summary>
    /// Tests that the contract response is obtained.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Fact]
    public async Task Test_WaitForMoreStates()
    {
        var contractor = new Contractor();
        var mock = new MockClass();

        var contract = new ContractBuilder<long, ContractMultipleStates, NoErrors>
                (contractor, ContractMultipleStates.None)
            .SetMoveAction(ContractMultipleStates.None, mock.Setup, ContractMultipleStates.Requested)
            .SetMoveFilter<ContractData<long>>
                (ContractMultipleStates.Requested, ContractMultipleStates.ResponseObtained)
            .SetFillData<ContractData<long>>(ContractMultipleStates.ResponseObtained, d => d.Data)
            .SetMoveFilter<ContractData<bool>>
                (ContractMultipleStates.ResponseObtained, c => c.Data, ContractMultipleStates.AfterResponseObtained)
            .Build();

        Task.Run
        (
            async () =>
            {
                await Task.Delay(500);
                await contractor.Update(new ContractData<long>(15));
                await Task.Delay(200);
                await contractor.Update(new ContractData<bool>(true));
            }
        );

        var result = await contract.WaitForAsync(ContractMultipleStates.AfterResponseObtained);
        result.IsSuccess.ShouldBeTrue();
        result.Entity.ShouldBe(15);
        contract.IsRegistered.ShouldBeFalse(); // trust the contract for now.
        contract.CurrentState.ShouldBe(ContractMultipleStates.AfterResponseObtained);
        mock.ExecutedTimes.ShouldBe(1);
    }

    /// <summary>
    /// Tests that the contract response is obtained.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Fact]
    public async Task Test_MoreStatesFollowed()
    {
        var contractor = new Contractor();
        var mock = new MockClass();

        var contract = new ContractBuilder<long, ContractMultipleStates, NoErrors>
                (contractor, ContractMultipleStates.None)
            .SetMoveAction(ContractMultipleStates.None, mock.Setup, ContractMultipleStates.Requested)
            .SetMoveFilter<ContractData<long>>
                (ContractMultipleStates.Requested, ContractMultipleStates.ResponseObtained)
            .SetFillData<ContractData<long>>(ContractMultipleStates.ResponseObtained, d => d.Data)
            .SetMoveFilter<ContractData<bool>>
                (ContractMultipleStates.ResponseObtained, c => c.Data, ContractMultipleStates.AfterResponseObtained)
            .Build();

        await contract.OnlyExecuteAsync();
        contract.Register();
        await contractor.Update(new ContractData<long>(15));
        await contractor.Update(new ContractData<bool>(true));
        contract.Unregister();
        var result = await contract.WaitForAsync(ContractMultipleStates.AfterResponseObtained);
        result.IsSuccess.ShouldBeTrue();
        result.Entity.ShouldBe(15);
        contract.IsRegistered.ShouldBeFalse(); // trust the contract for now.
        contract.CurrentState.ShouldBe(ContractMultipleStates.AfterResponseObtained);
        mock.ExecutedTimes.ShouldBe(1);
    }

    /// <summary>
    /// Tests that the contract response is obtained.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Fact]
    public async Task Test_WaitForTimeout()
    {
        var contractor = new Contractor();
        var mock = new MockClass();

        var contract = new ContractBuilder<long, ContractMultipleStates, NoErrors>
                (contractor, ContractMultipleStates.None)
            .SetMoveAction(ContractMultipleStates.None, mock.Setup, ContractMultipleStates.Requested)
            .SetMoveFilter<ContractData<long>>
                (ContractMultipleStates.Requested, ContractMultipleStates.ResponseObtained)
            .SetFillData<ContractData<long>>(ContractMultipleStates.ResponseObtained, d => d.Data)
            .SetTimeout
            (
                ContractMultipleStates.ResponseObtained,
                TimeSpan.FromMilliseconds(100),
                ContractMultipleStates.AfterResponseObtained
            )
            .Build();

        Task.Run
        (
            async () =>
            {
                await Task.Delay(500);
                await contractor.Update(new ContractData<long>(15));
            }
        );

        var result = await contract.WaitForAsync(ContractMultipleStates.AfterResponseObtained);
        result.IsSuccess.ShouldBeTrue();
        result.Entity.ShouldBe(15);
        contract.IsRegistered.ShouldBeFalse(); // trust the contract for now.
        contract.CurrentState.ShouldBe(ContractMultipleStates.AfterResponseObtained);
        mock.ExecutedTimes.ShouldBe(1);
    }

    /// <summary>
    /// Tests that the contract response is obtained.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Fact]
    public async Task Test_MultipleContracts()
    {
        var contractor = new Contractor();
        var mock = new MockClass();

        var contract1 = new ContractBuilder<long, ContractMultipleStates, NoErrors>
                (contractor, ContractMultipleStates.None)
            .SetMoveAction(ContractMultipleStates.None, mock.Setup, ContractMultipleStates.Requested)
            .SetMoveFilter<ContractData<long>>
            (
                ContractMultipleStates.Requested,
                d => d.Data > 10 && d.Data < 20,
                ContractMultipleStates.ResponseObtained
            )
            .SetFillData<ContractData<long>>(ContractMultipleStates.ResponseObtained, d => d.Data)
            .Build();

        var contract2 = new ContractBuilder<long, ContractMultipleStates, NoErrors>
                (contractor, ContractMultipleStates.None)
            .SetMoveAction(ContractMultipleStates.None, mock.Setup, ContractMultipleStates.Requested)
            .SetMoveFilter<ContractData<long>>
                (ContractMultipleStates.Requested, d => d.Data > 20, ContractMultipleStates.ResponseObtained)
            .SetFillData<ContractData<long>>(ContractMultipleStates.ResponseObtained, d => d.Data)
            .Build();

        await contract1.OnlyExecuteAsync();
        await contract2.OnlyExecuteAsync();

        Task.Run
        (
            async () =>
            {
                await Task.Delay(500);
                await contractor.Update(new ContractData<long>(15));
                await Task.Delay(500);
                await contractor.Update(new ContractData<long>(25));
            }
        );

        var results = await Task.WhenAll
        (
            contract1.WaitForAsync(ContractMultipleStates.ResponseObtained),
            contract2.WaitForAsync(ContractMultipleStates.ResponseObtained)
        );
        results[0].IsSuccess.ShouldBeTrue();
        results[0].Entity.ShouldBe(15);
        results[1].IsSuccess.ShouldBeTrue();
        results[1].Entity.ShouldBe(25);
        contract1.CurrentState.ShouldBe(ContractMultipleStates.ResponseObtained);
        contract2.CurrentState.ShouldBe(ContractMultipleStates.ResponseObtained);
        mock.ExecutedTimes.ShouldBe(2);
    }

    /// <summary>
    /// Tests that the contract response is obtained.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Fact]
    public async Task Test_ErrorsFired()
    {
        var contractor = new Contractor();
        var mock = new MockClass();

        var contract = new ContractBuilder<long, ContractMultipleStates, ContractError>
                (contractor, ContractMultipleStates.None)
            .SetMoveAction(ContractMultipleStates.None, mock.Setup, ContractMultipleStates.Requested)
            .SetMoveFilter<ContractData<long>>
                (ContractMultipleStates.Requested, ContractMultipleStates.ResponseObtained)
            .SetFillData<ContractData<long>>(ContractMultipleStates.ResponseObtained, d => d.Data)
            .SetError<ContractData<long>>
            (
                ContractMultipleStates.Requested,
                d =>
                {
                    if (d.Data == 15)
                    {
                        return ContractError.Error1;
                    }

                    return null;
                }
            )
            .Build();

        Task.Run
        (
            async () =>
            {
                await Task.Delay(500);
                await contractor.Update(new ContractData<long>(15));
            }
        );

        var result = await contract.WaitForAsync(ContractMultipleStates.AfterResponseObtained);
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldBeOfType<ContractError<ContractError>>();
        ((ContractError<ContractError>)result.Error).Error.ShouldBe(ContractError.Error1);
        contract.CurrentState.ShouldBe(ContractMultipleStates.Requested);
        contract.IsRegistered.ShouldBeFalse();
        mock.ExecutedTimes.ShouldBe(1);
    }
}

/// <summary>
/// A class for verifying setup was called.
/// </summary>
public class MockClass
{
    /// <summary>
    /// Gets the number of times <see cref="Setup"/> was called.
    /// </summary>
    public int ExecutedTimes { get; private set; }

    /// <summary>
    /// Gets whether <see cref="Executed"/> was executed.
    /// </summary>
    public bool Executed { get; private set; }

    /// <summary>
    /// Sets Executed to true..
    /// </summary>
    /// <param name="data">The data. should be null.</param>
    /// <param name="ct">The cancellation token used for cancelling the operation.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public Task<Result<bool>> Setup(object? data, CancellationToken ct)
    {
        if (data is not null)
        {
            throw new ArgumentException("Should be null.", nameof(data));
        }

        Executed = true;
        ExecutedTimes++;
        return Task.FromResult(Result<bool>.FromSuccess(true));
    }
}