## FDCardRepo

The `FDCardRepo` class is a partial class that extends the `BaseCardRepo` class. It is part of the HRBlock.Navigation.V2.FD.HubSpoke namespace.

### Constructor

The class has a default constructor that calls the base class constructor.

```csharp
public FDCardRepo() : base() { }
```

### PopulateRepo Method

The `PopulateRepo` method is an overridden method from the base class. It is responsible for populating the repository with data related to various aspects of the tax filing process.

```csharp
public override void PopulateRepo()
{
    PopulateBusiness();
    PopulateIncome();
    PopulateAdjustmentsAndDeductions();
    PopulateCredits();
    PopulateTaxes();
    this.IsCheckBoxItems = true;
}
```

The method calls several other methods to populate different sections of the repository, such as business information, income details, adjustments and deductions, credits, and taxes. After populating the repository, it sets the `IsCheckBoxItems` property to `true`.

Overall, the `FDCardRepo` class is used to populate the repository with data related to tax filing in the context of the FD (Filing and Deductions) module.

## FDCardRepo.PopulateBusiness

The `FDCardRepo.PopulateBusiness` method is responsible for populating the repository with data related to small businesses for tax filing in the FD module. It creates a `BusinessHub` object, which represents a group of cards related to small businesses. The `BusinessHub` object contains various properties such as the section title, tooltip, text, question, menu title, menu footer, and instructions.

Next, a `ScheduleCGroup` object is created, which represents a gateway group within the `BusinessHub`. The `ScheduleCGroup` contains properties such as the title, description, icon URL, sort order, and a condition to determine if it should be shown on the menu.

Then, a `CardModel<Form1040ScheduleC>` object is created, which represents a card for Schedule C income and expenses. The `CardModel` contains UI elements such as the card title, short description, long description, gateway caption, glossary ID, glossary tooltip, icon URL, start URL, delete topic description, and checklist name. It also contains tax data elements specific to the `Form1040ScheduleC` form, such as the amount of gross income.

The `CardModel` also has behaviors defined, such as determining if it should be shown on the hub, and an action to perform when the card is added. Additionally, the `CardModel` is associated with the `ScheduleCGroup`.

Finally, the `CardModel` is added to the repository using the `AddTo` method.

This method is part of the `FDCardRepo` class, which is a partial class that extends the `BaseCardRepo` class in the `HRBlock.Navigation.V2.FD.HubSpoke` namespace. The `FDCardRepo` class is used to populate the repository with data for tax filing in the FD module.
## FDCardRepo.PopulateCredits

The `FDCardRepo.PopulateCredits` method is responsible for populating the repository with data related to various tax credits. It creates a `HubGroup` object representing the credits hub, which contains properties such as the section title, tooltip, text, question, menu title, menu footer, and instructions.

Next, it creates several `GatewayGroup` objects representing different groups of credits within the hub. Each `GatewayGroup` contains properties such as the title, description, icon URL, sort order, and a condition to determine if it should be shown on the menu.

Then, it creates several `CardModel` objects representing individual cards for each credit. Each `CardModel` contains UI elements such as the card title, short description, long description, gateway caption, glossary ID, glossary tooltip, icon URL, start URL, delete topic description, and checklist name. It also contains behaviors defined for the card, such as determining if it should be shown on the hub, and actions to perform when the card is added or removed. Each `CardModel` is associated with a specific `GatewayGroup`.

Finally, the `CardModel` objects are added to the repository using the `AddTo` method.

This method is part of the `FDCardRepo` class, which is a partial class that extends the `BaseCardRepo` class in the `HRBlock.Navigation.V2.FD.HubSpoke` namespace. The `FDCardRepo` class is used to populate the repository with data for tax filing in the FD module.

## FDCardRepo.PopulateTaxes

The `FDCardRepo.PopulateTaxes` method is responsible for populating the repository with data related to various taxes, payments, and penalties. It creates a hub group called "Taxes" that contains cards related to different tax topics.

### Taxes Section Details

The `TaxesHub` object represents the hub group for taxes. It has properties such as the section title, tooltip, text, question, menu title, menu footer, and instructions. These properties provide information and guidance to the user regarding taxes.

### Taxes Menu Items

The method creates several gateway groups within the TaxesHub. Each gateway group represents a specific tax topic and contains properties such as the title, description, icon URL, sort order, and a condition to determine if it should be shown on the menu.

### Taxes Cards

For each tax topic, the method creates a card model representing an individual card. Each card model contains UI elements such as the card title, short description, long description, gateway caption, glossary ID, glossary tooltip, icon URL, start URL, delete topic description, and checklist name. It also contains behaviors defined for the card, such as determining if it should be shown on the hub and actions to perform when the card is added or removed. Each card model is associated with a specific gateway group.

Finally, the card models are added to the repository using the `AddTo` method.

The `FDCardRepo.PopulateTaxes` method is part of the `FDCardRepo` class, which is a partial class that extends the `BaseCardRepo` class in the `HRBlock.Navigation.V2.FD.HubSpoke` namespace. The `FDCardRepo` class is used to populate the repository with data for tax filing in the FD module.
