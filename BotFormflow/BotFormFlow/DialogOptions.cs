using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Connector;
using System;

using System.Collections.Generic;
using System.Linq;

#pragma warning disable 649


namespace BotFormFlow

{

    public enum DialogOptions

    {

        BLT, BlackForestHam, BuffaloChicken, ChickenAndBaconRanchMelt, ColdCutCombo, MeatballMarinara,

        OvenRoastedChicken, RoastBeef, RotisserieStyleChicken, SpicyItalian, SteakAndCheese, SweetOnionTeriyaki, Tuna,

        TurkeyBreast, Veggie

    };

    public enum LengthOptions { SixInch, FootLong };

    public enum BreadOptions { NineGrainWheat, NineGrainHoneyOat, Italian, ItalianHerbsAndCheese, Flatbread };

    public enum CheeseOptions { American, MontereyCheddar, Pepperjack };

    public enum ToppingOptions

    {
        [Terms("except", "but", "not", "no", "all", "everything")]

        Everything = 1,

        Avocado, BananaPeppers, Cucumbers, GreenBellPeppers, Jalapenos,

        Lettuce, Olives, Pickles, RedOnion, Spinach, Tomatoes

    };

    public enum SauceOptions

    {

        ChipotleSouthwest, HoneyMustard, LightMayonnaise, RegularMayonnaise,

        Mustard, Oil, Pepper, Ranch, SweetOnion, Vinegar

    };



    [Serializable]
    [Template(TemplateUsage.EnumSelectOne, "What kind of {&} would you like on your sandwich? {||}", ChoiceStyle = ChoiceStyleOptions.PerLine)]

    public class SandwichOrder

    {
        //[Prompt("What kind of {&} would you like? {||}")]
        [Prompt("What kind of {&} would you like? {||}", ChoiceFormat = "{1}")]
        public DialogOptions? Sandwich;

        public LengthOptions? Length;

        public BreadOptions? Bread;

        public CheeseOptions? Cheese;

        public List<ToppingOptions> Toppings;

        [Optional]
        public List<SauceOptions> Sauce;
        [Pattern(@"(\d)?\s*\d{3}(-|\s*)\d{4}")]

        public string PhoneNumber;

        public int DeliveryTime;

        public int Cost;

        public static IForm<SandwichOrder> BuildForm()

        {
            OnCompletionAsyncDelegate<SandwichOrder> processOrder = async (context, state) =>

            {  
                await context.PostAsync("We are currently processing your sandwich. We will message you the status.");

            };

            return new FormBuilder<SandwichOrder>()

                    .Message("Welcome to the simple sandwich order bot!")
                    .Field(nameof(Cheese))
                    .Field(nameof(Sandwich))
                    .Field(nameof(Length))
                    .Field(nameof(Bread))
                    .Field(nameof(Sauce))
                    .Field(nameof(PhoneNumber))
                    .Field(nameof(DeliveryTime)) 
                    //.Field(nameof(Toppings),
                    //        validate: async (state, value) =>
                    //        {
                    //            var values = ((List<object>)value).OfType<ToppingOptions>();
                    //            var result = new ValidateResult { IsValid = true, Value = values };
                    //            if (values != null && values.Contains(ToppingOptions.Everything))                                {
                    //                result.Value = (from ToppingOptions topping in Enum.GetValues(typeof(ToppingOptions))
                    //                                where topping != ToppingOptions.Everything && !values.Contains(topping)
                    //                                select topping).ToList();
                    //            }
                    //            return result;
                    //        })
                    //   .Field(nameof(SandwichOrder.DeliveryTime), "What time do you want your sandwich delivered? {||}")

                    //    .Confirm("Do you want to order your {Sandwich} on {Bread} to be sent to {DeliveryTime}?")
                    //    .Confirm(async (state) =>
                    //    {
                    //        var cost = 5.0;
                    //        return new PromptAttribute($"Total for your sandwich is {cost:C2} is that ok?");
                    //    })
                        .AddRemainingFields()
                        .Message("Thanks for ordering a sandwich!")
                        .OnCompletion(processOrder)
                        .Message("For sandwich toppings you have selected {Toppings}.")
                    .Build();
        }

    };

}

