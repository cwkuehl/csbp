// <copyright file="CsbpInjector.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Factory
{
  using CSBP.Apis.Services;
  using CSBP.Services.Base;

  public class CsbpInjector : AbstractModule
  {
    public override void Configure()
    {
      // Services
      Bind<IAddressService, AddressService>();
      Bind<IBudgetService, BudgetService>();
      Bind<IClientService, ClientService>();
      Bind<IDiaryService, DiaryService>();
      Bind<ILoginService, LoginService>();
      Bind<IPedigreeService, PedigreeService>();
      Bind<IPrivateService, PrivateService>();
      Bind<IStockService, StockService>();
    }
  }
}
