// <copyright file="FactoryService.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Factory;

using CSBP.Services.Apis.Services;
using CSBP.Services.Base;

/// <summary>
/// Factory creates service instances.
/// </summary>
public class FactoryService
{
  /// <summary>Injector for csbp services.</summary>
  private static readonly Injector Injector = Injector.Create(new CsbpInjector());

  /// <summary>Gets instance of address service.</summary>
  public static IAddressService AddressService => Injector.GetInstance<IAddressService>();

  /// <summary>Gets instance of address service.</summary>
  public static IBudgetService BudgetService => Injector.GetInstance<IBudgetService>();

  /// <summary>Gets instance of client service.</summary>
  public static IClientService ClientService => Injector.GetInstance<IClientService>();

  /// <summary>Gets instance of diary service.</summary>
  public static IDiaryService DiaryService => Injector.GetInstance<IDiaryService>();

  /// <summary>Gets instance of login service.</summary>
  public static ILoginService LoginService => Injector.GetInstance<ILoginService>();

  /// <summary>Gets instance of pedigree service.</summary>
  public static IPedigreeService PedigreeService => Injector.GetInstance<IPedigreeService>();

  /// <summary>Gets instance of private service.</summary>
  public static IPrivateService PrivateService => Injector.GetInstance<IPrivateService>();

  /// <summary>Gets instance of stock service.</summary>
  public static IStockService StockService => Injector.GetInstance<IStockService>();
}
