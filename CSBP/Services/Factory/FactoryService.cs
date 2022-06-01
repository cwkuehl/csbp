// <copyright file="FactoryService.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Factory
{
  using CSBP.Apis.Services;
  using CSBP.Services.Base;

  public class FactoryService
  {
    private static readonly Injector injector = Injector.Create(new CsbpInjector());

    public static IAddressService AddressService => injector.GetInstance<IAddressService>();
    public static IBudgetService BudgetService => injector.GetInstance<IBudgetService>();
    public static IClientService ClientService => injector.GetInstance<IClientService>();
    public static IDiaryService DiaryService => injector.GetInstance<IDiaryService>();
    public static ILoginService LoginService => injector.GetInstance<ILoginService>();
    public static IPedigreeService PedigreeService => injector.GetInstance<IPedigreeService>();
    public static IPrivateService PrivateService => injector.GetInstance<IPrivateService>();
    public static IStockService StockService => injector.GetInstance<IStockService>();
  }
}
