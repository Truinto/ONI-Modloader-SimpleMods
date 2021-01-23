

            Debug.Log("DEBUG HELLO 8");

            if (setting.lifespan != null)
            {
                Debug.Log(setting.id + ".lifespan: This value is not read from the game. It exists only as artifact. Use traits instead!");
                //trait.Add(new AttributeModifier(Db.Get().Amounts.Age.maxAttribute.Id, 150f, name, false, false, true));
            }

            Debug.Log("DEBUG HELLO 27");



            Debug.Log("DEBUG HELLO 29");



            Debug.Log("DEBUG HELLO 30");

            if (setting.adultId != null)
            {
                go.AddOrGetDef<BabyMonitor.Def>().adultPrefab = setting.adultId;
            }