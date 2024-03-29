﻿using System.Collections.Generic;
using Common;

namespace CustomizeCritter
{
    public class CustomizeCritterState
    {
        public int version = 1;

        public bool debug = true;

        public bool print_all = true;
        public bool print_verbose = false;

        public List<CritterContainer> critter_settings = new();
        public List<EggContainer> egg_settings = new();
        public List<TraitContainer> trait_settings = new();

        public bool clear_vanilla_egg_modifiers = false;

        public List<EggModifiers> egg_modifiers = new();

        public List<TraitContainer.Attribute> wildEffect = new()
        {
                new TraitContainer.Attribute( "WildnessDelta", 0.008333334f, false ),
                new TraitContainer.Attribute( "Metabolism", 25f, false ),
                new TraitContainer.Attribute( "ScaleGrowthDelta", -0.75f, true )
        };
        public List<TraitContainer.Attribute> tameEffect = new()
        {
                new TraitContainer.Attribute( "Happiness", -1f, false ),
                new TraitContainer.Attribute( "Metabolism", 100f, false )
        };

        public List<TraitContainer.Attribute> happyWildEffect = new() { };
        public List<TraitContainer.Attribute> happyTameEffect = new()
        {
                new TraitContainer.Attribute( "FertilityDelta", 9f, true )
        };
        public List<TraitContainer.Attribute> unhappyWildEffect = new()
        {
                new TraitContainer.Attribute( "Metabolism", -15f, false )
        };
        public List<TraitContainer.Attribute> unhappyTameEffect = new()
        {
                new TraitContainer.Attribute( "Metabolism", -80f, false )
                //, new TraitContainer.Attribute( "WildnessDelta", 0.008333334f, false )
        };


        public bool alwaysHungry = true;
        public bool cantStarve = true;
        public float eggWildness = -1f;
        public float babyGrowupTime_inDays = 5f;
        public bool acceleratedLifecycle = true;

        public static Config.Manager<CustomizeCritterState> StateManager = new(Config.PathHelper.CreatePath("CustomizeCritter"), true);
    }
}