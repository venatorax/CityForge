using Colossal;
using Colossal.IO.AssetDatabase;
using Game.Modding;
using Game.Settings;
using System.Collections.Generic;

namespace CityForge
{
    [FileLocation("ModsSettings/CityForge/CityForge")]
    [SettingsUIGroupOrder(kMainGroup)]
    [SettingsUIShowGroupName(kMainGroup)]
    public class Setting : ModSetting
    {
        public const string kSection = "Main";
        public const string kMainGroup = "General";

        [SettingsUISection(kSection, kMainGroup)]
        public bool ShowToolbarButton { get; set; } = true;

        [SettingsUIHidden] public bool InfiniteMoney { get; set; } = false;
        [SettingsUIHidden] public bool KeepMilestonesUnlocked { get; set; } = false;
        [SettingsUIHidden] public int TargetMilestone { get; set; } = 3;

        [SettingsUIHidden] public bool OverrideResidentialDemand { get; set; } = false;
        [SettingsUIHidden] public int ResidentialDemandLow { get; set; } = 50;
        [SettingsUIHidden] public int ResidentialDemandMedium { get; set; } = 50;
        [SettingsUIHidden] public int ResidentialDemandHigh { get; set; } = 50;
        [SettingsUIHidden] public bool ForceResidentialBuild { get; set; } = false;
        [SettingsUIHidden] public bool OverrideCommercialDemand { get; set; } = false;
        [SettingsUIHidden] public int CommercialDemandValue { get; set; } = 50;
        [SettingsUIHidden] public bool ForceCommercialBuild { get; set; } = false;
        [SettingsUIHidden] public bool OverrideIndustrialDemand { get; set; } = false;
        [SettingsUIHidden] public int IndustrialDemandValue { get; set; } = 50;
        [SettingsUIHidden] public bool ForceIndustrialBuild { get; set; } = false;
        [SettingsUIHidden] public bool OverrideOfficeDemand { get; set; } = false;
        [SettingsUIHidden] public int OfficeDemandValue { get; set; } = 50;
        [SettingsUIHidden] public bool ForceOfficeBuild { get; set; } = false;

        [SettingsUIHidden] public bool InstantConstruction { get; set; } = false;
        [SettingsUIHidden] public int ConstructionSpeedIndex { get; set; } = 1;

        [SettingsUIHidden] public bool OverrideMoveIn { get; set; } = false;
        [SettingsUIHidden] public int MoveInMultiplier { get; set; } = 1;
        [SettingsUIHidden] public bool OverrideTourists { get; set; } = false;
        [SettingsUIHidden] public int TouristMultiplier { get; set; } = 1;

        [SettingsUIHidden] public bool MaxHappiness { get; set; } = false;
        [SettingsUIHidden] public bool RichCitizens { get; set; } = false;
        [SettingsUIHidden] public bool MaxCompanyEfficiency { get; set; } = false;

        [SettingsUIHidden] public bool ResetOnNewMap { get; set; } = true;

        public Setting(IMod mod) : base(mod) { }

        public void ResetCheats()
        {
            InfiniteMoney = false;
            KeepMilestonesUnlocked = false;
            OverrideResidentialDemand = false;
            ForceResidentialBuild = false;
            OverrideCommercialDemand = false;
            ForceCommercialBuild = false;
            OverrideIndustrialDemand = false;
            ForceIndustrialBuild = false;
            OverrideOfficeDemand = false;
            ForceOfficeBuild = false;
            InstantConstruction = false;
            ConstructionSpeedIndex = 1;
            OverrideMoveIn = false;
            MoveInMultiplier = 1;
            OverrideTourists = false;
            TouristMultiplier = 1;
            MaxHappiness = false;
            RichCitizens = false;
            MaxCompanyEfficiency = false;
        }

        public override void SetDefaults()
        {
            ShowToolbarButton = true;
            InfiniteMoney = false;
            KeepMilestonesUnlocked = false;
            TargetMilestone = 3;
            OverrideResidentialDemand = false;
            ResidentialDemandLow = 50;
            ResidentialDemandMedium = 50;
            ResidentialDemandHigh = 50;
            ForceResidentialBuild = false;
            OverrideCommercialDemand = false;
            CommercialDemandValue = 50;
            ForceCommercialBuild = false;
            OverrideIndustrialDemand = false;
            IndustrialDemandValue = 50;
            ForceIndustrialBuild = false;
            OverrideOfficeDemand = false;
            OfficeDemandValue = 50;
            ForceOfficeBuild = false;
            InstantConstruction = false;
            ConstructionSpeedIndex = 1;
            OverrideMoveIn = false;
            MoveInMultiplier = 1;
            OverrideTourists = false;
            TouristMultiplier = 1;
            MaxHappiness = false;
            RichCitizens = false;
            MaxCompanyEfficiency = false;
        }

        public class LocaleEN : IDictionarySource
        {
            private readonly Setting _s;
            public LocaleEN(Setting s) => _s = s;
            public IEnumerable<KeyValuePair<string, string>> ReadEntries(IList<IDictionaryEntryError> e, Dictionary<string, int> i) =>
                new Dictionary<string, string>
                {
                    { _s.GetSettingsLocaleID(),                             "CityForge"                             },
                    { _s.GetOptionTabLocaleID(kSection),                    "Settings"                             },
                    { _s.GetOptionGroupLocaleID(kMainGroup),                "General"                              },
                    { _s.GetOptionLabelLocaleID(nameof(ShowToolbarButton)), "Show toolbar button"                  },
                    { _s.GetOptionDescLocaleID(nameof(ShowToolbarButton)),  "Show CityForge button in the toolbar." },
                    { "CheatMod.SECTION[Money]",            "Money"                        },
                    { "CheatMod.OPTION[InfiniteMoney]",     "Infinite Money"               },
                    { "CheatMod.SECTION[Milestones]",       "Milestones"                   },
                    { "CheatMod.ACTION[UnlockAll]",         "Unlock All"                   },
                    { "CheatMod.ACTION[AdvanceToTarget]",   "Advance to"                   },
                    { "CheatMod.LABEL[TargetMilestone]",    "Target"                       },
                    { "CheatMod.OPTION[KeepUnlocked]",      "Keep permanently unlocked"    },
                    { "CheatMod.SECTION[DevTree]",          "Development Tree"             },
                    { "CheatMod.ACTION[AddPoints]",         "+ 228 Points"                 },
                    { "CheatMod.SECTION[Demand]",           "Demand"                       },
                    { "CheatMod.DEMAND[Residential]",       "Residential"                  },
                    { "CheatMod.DEMAND[Commercial]",        "Commercial"                   },
                    { "CheatMod.DEMAND[Industrial]",        "Industrial"                   },
                    { "CheatMod.DEMAND[Office]",            "Office"                       },
                    { "CheatMod.LEVEL[Low]",                "Low"                          },
                    { "CheatMod.LEVEL[Medium]",             "Medium"                       },
                    { "CheatMod.LEVEL[High]",               "High"                         },
                    { "CheatMod.ACTION[ForceBuild]",        "Force Build"                  },
                    { "CheatMod.SECTION[Construction]",     "Construction Time"            },
                    { "CheatMod.OPTION[InstantBuild]",      "Instant Construction"         },
                    { "CheatMod.LABEL[BuildSpeed]",         "Build Speed"                  },
                    { "CheatMod.SECTION[Population]",       "Population & Tourism"         },
                    { "CheatMod.OPTION[MoveIn]",            "Boost Move-In Rate"           },
                    { "CheatMod.LABEL[MoveInMult]",         "Move-In Multiplier"           },
                    { "CheatMod.OPTION[Tourists]",          "Boost Tourists"               },
                    { "CheatMod.LABEL[TouristMult]",        "Tourist Multiplier"           },
                    { "CheatMod.OPTION[MaxHappiness]",      "Max Citizen Happiness"        },
                    { "CheatMod.OPTION[RichCitizens]",      "Rich Citizens"                },
                    { "CheatMod.SECTION[Buildings]",        "Buildings"                    },
                    { "CheatMod.ACTION[UpgradeAll]",        "Upgrade All to Level 5"       },
                    { "CheatMod.OPTION[MaxEfficiency]",     "Max Company Efficiency"       },
                    { "CheatMod.ACTION[FillStorage]",       "Fill Storage"                 },
                    { "CheatMod.OPTION[KeepStorageFull]",   "Keep storage permanently full"},
                    { "CheatMod.OPTION[ResetOnNewMap]",     "Reset cheats on new map"      },
                    { "CheatMod.INFO[InstantBuild]",        "Instant build active \u2014 buildings are completed immediately"     },
                    { "CheatMod.INFO[Happiness]",           "All citizens have maximum happiness (200)"                          },
                    { "CheatMod.INFO[RichCitizens]",        "All households receive 500,000 money (every 5 game minutes)"        },
                    { "CheatMod.INFO[Efficiency]",          "Company efficiency is maximized"                                    },
                    { "CheatMod.INFO[ResetOnNewMap]",       "All cheats will be disabled on next map load"                       },
                    { "CheatMod.INFO[ForceBuild]",          "Force Build active \u2014 Unlimited demand"                         },
                    { "CheatMod.INFO[UpgradeLoading]",      "Loading\u2026"                                                      },
                    { "CheatMod.HINT[Drag]",                "Drag title \u00b7 corner to resize"                                 },
                };
            public void Unload() { }
        }

        public class LocaleDE : IDictionarySource
        {
            private readonly Setting _s;
            public LocaleDE(Setting s) => _s = s;
            public IEnumerable<KeyValuePair<string, string>> ReadEntries(IList<IDictionaryEntryError> e, Dictionary<string, int> i) =>
                new Dictionary<string, string>
                {
                    { _s.GetSettingsLocaleID(),                             "CityForge"                                  },
                    { _s.GetOptionTabLocaleID(kSection),                    "Einstellungen"                             },
                    { _s.GetOptionGroupLocaleID(kMainGroup),                "Allgemein"                                 },
                    { _s.GetOptionLabelLocaleID(nameof(ShowToolbarButton)), "Toolbar-Button anzeigen"                   },
                    { _s.GetOptionDescLocaleID(nameof(ShowToolbarButton)),  "Zeigt den CityForge-Button in der Toolbar." },
                    { "CheatMod.SECTION[Money]",            "Geld"                                    },
                    { "CheatMod.OPTION[InfiniteMoney]",     "Unendlich Geld"                          },
                    { "CheatMod.SECTION[Milestones]",       "Meilensteine"                            },
                    { "CheatMod.ACTION[UnlockAll]",         "Alle freischalten"                       },
                    { "CheatMod.ACTION[AdvanceToTarget]",   "Voranschreiten bis"                      },
                    { "CheatMod.LABEL[TargetMilestone]",    "Ziel-Meilenstein"                        },
                    { "CheatMod.OPTION[KeepUnlocked]",      "Dauerhaft entsperrt halten"              },
                    { "CheatMod.SECTION[DevTree]",          "Entwicklungsbaum"                        },
                    { "CheatMod.ACTION[AddPoints]",         "+ 228 Punkte"                            },
                    { "CheatMod.SECTION[Demand]",           "Nachfrage"                               },
                    { "CheatMod.DEMAND[Residential]",       "Wohngeb\u00e4ude"                        },
                    { "CheatMod.DEMAND[Commercial]",        "Gewerbe"                                 },
                    { "CheatMod.DEMAND[Industrial]",        "Industrie"                               },
                    { "CheatMod.DEMAND[Office]",            "B\u00fcro"                               },
                    { "CheatMod.LEVEL[Low]",                "Niedrig"                                 },
                    { "CheatMod.LEVEL[Medium]",             "Mittel"                                  },
                    { "CheatMod.LEVEL[High]",               "Hoch"                                    },
                    { "CheatMod.ACTION[ForceBuild]",        "Sofort bauen"                            },
                    { "CheatMod.SECTION[Construction]",     "Bauzeit"                                 },
                    { "CheatMod.OPTION[InstantBuild]",      "Sofortiger Bau"                          },
                    { "CheatMod.LABEL[BuildSpeed]",         "Baugeschwindigkeit"                      },
                    { "CheatMod.SECTION[Population]",       "Bev\u00f6lkerung & Tourismus"            },
                    { "CheatMod.OPTION[MoveIn]",            "Einzugsrate erh\u00f6hen"                },
                    { "CheatMod.LABEL[MoveInMult]",         "Einzugs-Multiplikator"                   },
                    { "CheatMod.OPTION[Tourists]",          "Touristen erh\u00f6hen"                  },
                    { "CheatMod.LABEL[TouristMult]",        "Touristen-Multiplikator"                 },
                    { "CheatMod.OPTION[MaxHappiness]",      "Maximale B\u00fcrgerzufriedenheit"        },
                    { "CheatMod.OPTION[RichCitizens]",      "Reiche B\u00fcrger"                      },
                    { "CheatMod.SECTION[Buildings]",        "Geb\u00e4ude"                            },
                    { "CheatMod.ACTION[UpgradeAll]",        "Alle auf Stufe 5 upgraden"               },
                    { "CheatMod.OPTION[MaxEfficiency]",     "Max. Unternehmenseffizienz"              },
                    { "CheatMod.ACTION[FillStorage]",       "Lager auff\u00fcllen"                    },
                    { "CheatMod.OPTION[KeepStorageFull]",   "Lager dauerhaft voll halten"             },
                    { "CheatMod.OPTION[ResetOnNewMap]",     "Cheats bei neuer Map zur\u00fccksetzen"  },
                    { "CheatMod.INFO[InstantBuild]",        "Sofortiger Bau aktiv \u2014 Geb\u00e4ude werden direkt fertiggestellt"         },
                    { "CheatMod.INFO[Happiness]",           "Alle B\u00fcrger haben maximale Zufriedenheit (200)"                          },
                    { "CheatMod.INFO[RichCitizens]",        "Alle Haushalte erhalten 500.000 Geld (alle 5 Spielminuten)"                   },
                    { "CheatMod.INFO[Efficiency]",          "Unternehmenseffizienz wird maximiert"                                         },
                    { "CheatMod.INFO[ResetOnNewMap]",       "Beim n\u00e4chsten Mapstart werden alle Cheats ausgeschaltet"                 },
                    { "CheatMod.INFO[ForceBuild]",          "Sofort-Bau aktiv \u2014 Unbegrenzte Nachfrage"                               },
                    { "CheatMod.INFO[UpgradeLoading]",      "Wird geladen\u2026"                                                           },
                    { "CheatMod.HINT[Drag]",                "Titel ziehen \u00b7 Ecke zum Skalieren"                                       },
                };
            public void Unload() { }
        }

        public class LocaleFR : IDictionarySource
        {
            private readonly Setting _s;
            public LocaleFR(Setting s) => _s = s;
            public IEnumerable<KeyValuePair<string, string>> ReadEntries(IList<IDictionaryEntryError> e, Dictionary<string, int> i) =>
                new Dictionary<string, string>
                {
                    { _s.GetSettingsLocaleID(),                             "CityForge"                                     },
                    { _s.GetOptionTabLocaleID(kSection),                    "Param\u00e8tres"                              },
                    { _s.GetOptionGroupLocaleID(kMainGroup),                "G\u00e9n\u00e9ral"                            },
                    { _s.GetOptionLabelLocaleID(nameof(ShowToolbarButton)), "Afficher bouton barre d'outils"               },
                    { _s.GetOptionDescLocaleID(nameof(ShowToolbarButton)),  "Affiche le bouton CityForge dans la barre."    },
                    { "CheatMod.SECTION[Money]",            "Argent"                                  },
                    { "CheatMod.OPTION[InfiniteMoney]",     "Argent infini"                           },
                    { "CheatMod.SECTION[Milestones]",       "Jalons"                                  },
                    { "CheatMod.ACTION[UnlockAll]",         "Tout d\u00e9verrouiller"                 },
                    { "CheatMod.ACTION[AdvanceToTarget]",   "Avancer jusqu'\u00e0"                    },
                    { "CheatMod.LABEL[TargetMilestone]",    "Jalon cible"                             },
                    { "CheatMod.OPTION[KeepUnlocked]",      "Garder d\u00e9verrouill\u00e9"           },
                    { "CheatMod.SECTION[DevTree]",          "Arbre de d\u00e9veloppement"             },
                    { "CheatMod.ACTION[AddPoints]",         "+ 228 Points"                            },
                    { "CheatMod.SECTION[Demand]",           "Demande"                                 },
                    { "CheatMod.DEMAND[Residential]",       "R\u00e9sidentiel"                        },
                    { "CheatMod.DEMAND[Commercial]",        "Commercial"                              },
                    { "CheatMod.DEMAND[Industrial]",        "Industriel"                              },
                    { "CheatMod.DEMAND[Office]",            "Bureau"                                  },
                    { "CheatMod.LEVEL[Low]",                "Bas"                                     },
                    { "CheatMod.LEVEL[Medium]",             "Moyen"                                   },
                    { "CheatMod.LEVEL[High]",               "\u00c9lev\u00e9"                         },
                    { "CheatMod.ACTION[ForceBuild]",        "Construction forc\u00e9e"                },
                    { "CheatMod.SECTION[Construction]",     "Temps de construction"                   },
                    { "CheatMod.OPTION[InstantBuild]",      "Construction instantan\u00e9e"           },
                    { "CheatMod.LABEL[BuildSpeed]",         "Vitesse de construction"                 },
                    { "CheatMod.SECTION[Population]",       "Population & Tourisme"                   },
                    { "CheatMod.OPTION[MoveIn]",            "Acc\u00e9l\u00e9rer l'emm\u00e9nagement" },
                    { "CheatMod.LABEL[MoveInMult]",         "Multiplicateur d'emm\u00e9nagement"      },
                    { "CheatMod.OPTION[Tourists]",          "Augmenter les touristes"                 },
                    { "CheatMod.LABEL[TouristMult]",        "Multiplicateur de touristes"             },
                    { "CheatMod.OPTION[MaxHappiness]",      "Bonheur maximum"                         },
                    { "CheatMod.OPTION[RichCitizens]",      "Citoyens riches"                         },
                    { "CheatMod.SECTION[Buildings]",        "B\u00e2timents"                          },
                    { "CheatMod.ACTION[UpgradeAll]",        "Tout upgrader niveau 5"                  },
                    { "CheatMod.OPTION[MaxEfficiency]",     "Efficacit\u00e9 max entreprises"         },
                    { "CheatMod.ACTION[FillStorage]",       "Remplir le stockage"                     },
                    { "CheatMod.OPTION[KeepStorageFull]",   "Maintenir le stockage plein"             },
                    { "CheatMod.OPTION[ResetOnNewMap]",     "R\u00e9initialiser les cheats sur nouvelle carte" },
                    { "CheatMod.INFO[InstantBuild]",        "Construction instantan\u00e9e active \u2014 b\u00e2timents termin\u00e9s imm\u00e9diatement" },
                    { "CheatMod.INFO[Happiness]",           "Tous les citoyens ont le bonheur maximum (200)"                                                },
                    { "CheatMod.INFO[RichCitizens]",        "Tous les m\u00e9nages re\u00e7oivent 500 000 d'argent (toutes les 5 minutes de jeu)"          },
                    { "CheatMod.INFO[Efficiency]",          "Efficacit\u00e9 des entreprises maximis\u00e9e"                                               },
                    { "CheatMod.INFO[ResetOnNewMap]",       "Tous les cheats seront d\u00e9sactiv\u00e9s au prochain chargement de carte"                  },
                    { "CheatMod.INFO[ForceBuild]",          "Construction forc\u00e9e active \u2014 Demande illimit\u00e9e"                                },
                    { "CheatMod.INFO[UpgradeLoading]",      "Chargement\u2026"                                                                             },
                    { "CheatMod.HINT[Drag]",                "Glisser titre \u00b7 coin pour redimensionner"                                                },
                };
            public void Unload() { }
        }

        public class LocaleES : IDictionarySource
        {
            private readonly Setting _s;
            public LocaleES(Setting s) => _s = s;
            public IEnumerable<KeyValuePair<string, string>> ReadEntries(IList<IDictionaryEntryError> e, Dictionary<string, int> i) =>
                new Dictionary<string, string>
                {
                    { _s.GetSettingsLocaleID(),                             "CityForge"                                       },
                    { _s.GetOptionTabLocaleID(kSection),                    "Ajustes"                                        },
                    { _s.GetOptionGroupLocaleID(kMainGroup),                "General"                                        },
                    { _s.GetOptionLabelLocaleID(nameof(ShowToolbarButton)), "Mostrar bot\u00f3n de barra"                    },
                    { _s.GetOptionDescLocaleID(nameof(ShowToolbarButton)),  "Muestra el bot\u00f3n de CityForge en la barra." },
                    { "CheatMod.SECTION[Money]",            "Dinero"                              },
                    { "CheatMod.OPTION[InfiniteMoney]",     "Dinero infinito"                     },
                    { "CheatMod.SECTION[Milestones]",       "Hitos"                               },
                    { "CheatMod.ACTION[UnlockAll]",         "Desbloquear todo"                    },
                    { "CheatMod.ACTION[AdvanceToTarget]",   "Avanzar hasta"                       },
                    { "CheatMod.LABEL[TargetMilestone]",    "Meta"                                },
                    { "CheatMod.OPTION[KeepUnlocked]",      "Mantener desbloqueado"               },
                    { "CheatMod.SECTION[DevTree]",          "\u00c1rbol de desarrollo"            },
                    { "CheatMod.ACTION[AddPoints]",         "+ 228 Puntos"                        },
                    { "CheatMod.SECTION[Demand]",           "Demanda"                             },
                    { "CheatMod.DEMAND[Residential]",       "Residencial"                         },
                    { "CheatMod.DEMAND[Commercial]",        "Comercial"                           },
                    { "CheatMod.DEMAND[Industrial]",        "Industrial"                          },
                    { "CheatMod.DEMAND[Office]",            "Oficina"                             },
                    { "CheatMod.LEVEL[Low]",                "Bajo"                                },
                    { "CheatMod.LEVEL[Medium]",             "Medio"                               },
                    { "CheatMod.LEVEL[High]",               "Alto"                                },
                    { "CheatMod.ACTION[ForceBuild]",        "Construcci\u00f3n forzada"           },
                    { "CheatMod.SECTION[Construction]",     "Tiempo de construcci\u00f3n"         },
                    { "CheatMod.OPTION[InstantBuild]",      "Construcci\u00f3n instant\u00e1nea"  },
                    { "CheatMod.LABEL[BuildSpeed]",         "Velocidad de construcci\u00f3n"      },
                    { "CheatMod.SECTION[Population]",       "Poblaci\u00f3n y Turismo"            },
                    { "CheatMod.OPTION[MoveIn]",            "Aumentar entrada de residentes"      },
                    { "CheatMod.LABEL[MoveInMult]",         "Multiplicador de entrada"            },
                    { "CheatMod.OPTION[Tourists]",          "Aumentar turistas"                   },
                    { "CheatMod.LABEL[TouristMult]",        "Multiplicador de turistas"           },
                    { "CheatMod.OPTION[MaxHappiness]",      "Felicidad m\u00e1xima ciudadana"     },
                    { "CheatMod.OPTION[RichCitizens]",      "Ciudadanos ricos"                    },
                    { "CheatMod.SECTION[Buildings]",        "Edificios"                           },
                    { "CheatMod.ACTION[UpgradeAll]",        "Mejorar todo a nivel 5"              },
                    { "CheatMod.OPTION[MaxEfficiency]",     "Eficiencia m\u00e1x empresas"        },
                    { "CheatMod.ACTION[FillStorage]",       "Llenar almac\u00e9n"                 },
                    { "CheatMod.OPTION[KeepStorageFull]",   "Mantener almac\u00e9n siempre lleno" },
                    { "CheatMod.OPTION[ResetOnNewMap]",     "Reiniciar trucos en nuevo mapa"      },
                    { "CheatMod.INFO[InstantBuild]",        "Construcci\u00f3n instant\u00e1nea activa \u2014 los edificios se completan de inmediato" },
                    { "CheatMod.INFO[Happiness]",           "Todos los ciudadanos tienen felicidad m\u00e1xima (200)"                               },
                    { "CheatMod.INFO[RichCitizens]",        "Todos los hogares reciben 500.000 de dinero (cada 5 minutos de juego)"                 },
                    { "CheatMod.INFO[Efficiency]",          "Eficiencia de empresas maximizada"                                                     },
                    { "CheatMod.INFO[ResetOnNewMap]",       "Todos los trucos se desactivar\u00e1n al cargar un nuevo mapa"                        },
                    { "CheatMod.INFO[ForceBuild]",          "Construcci\u00f3n forzada activa \u2014 Demanda ilimitada"                            },
                    { "CheatMod.INFO[UpgradeLoading]",      "Cargando\u2026"                                                                        },
                    { "CheatMod.HINT[Drag]",                "Arrastra t\u00edtulo \u00b7 esquina para redimensionar"                               },
                };
            public void Unload() { }
        }

        public class LocaleIT : IDictionarySource
        {
            private readonly Setting _s;
            public LocaleIT(Setting s) => _s = s;
            public IEnumerable<KeyValuePair<string, string>> ReadEntries(IList<IDictionaryEntryError> e, Dictionary<string, int> i) =>
                new Dictionary<string, string>
                {
                    { _s.GetSettingsLocaleID(),                             "CityForge"                                       },
                    { _s.GetOptionTabLocaleID(kSection),                    "Impostazioni"                                   },
                    { _s.GetOptionGroupLocaleID(kMainGroup),                "Generale"                                       },
                    { _s.GetOptionLabelLocaleID(nameof(ShowToolbarButton)), "Mostra pulsante barra strumenti"                },
                    { _s.GetOptionDescLocaleID(nameof(ShowToolbarButton)),  "Mostra il pulsante CityForge nella barra."       },
                    { "CheatMod.SECTION[Money]",            "Denaro"                              },
                    { "CheatMod.OPTION[InfiniteMoney]",     "Denaro infinito"                     },
                    { "CheatMod.SECTION[Milestones]",       "Traguardi"                           },
                    { "CheatMod.ACTION[UnlockAll]",         "Sblocca tutto"                       },
                    { "CheatMod.ACTION[AdvanceToTarget]",   "Avanza fino a"                       },
                    { "CheatMod.LABEL[TargetMilestone]",    "Obiettivo"                           },
                    { "CheatMod.OPTION[KeepUnlocked]",      "Mantieni sbloccato"                  },
                    { "CheatMod.SECTION[DevTree]",          "Albero di sviluppo"                  },
                    { "CheatMod.ACTION[AddPoints]",         "+ 228 Punti"                         },
                    { "CheatMod.SECTION[Demand]",           "Domanda"                             },
                    { "CheatMod.DEMAND[Residential]",       "Residenziale"                        },
                    { "CheatMod.DEMAND[Commercial]",        "Commerciale"                         },
                    { "CheatMod.DEMAND[Industrial]",        "Industriale"                         },
                    { "CheatMod.DEMAND[Office]",            "Ufficio"                             },
                    { "CheatMod.LEVEL[Low]",                "Basso"                               },
                    { "CheatMod.LEVEL[Medium]",             "Medio"                               },
                    { "CheatMod.LEVEL[High]",               "Alto"                                },
                    { "CheatMod.ACTION[ForceBuild]",        "Costruzione forzata"                 },
                    { "CheatMod.SECTION[Construction]",     "Tempo di costruzione"                },
                    { "CheatMod.OPTION[InstantBuild]",      "Costruzione istantanea"              },
                    { "CheatMod.LABEL[BuildSpeed]",         "Velocit\u00e0 di costruzione"        },
                    { "CheatMod.SECTION[Population]",       "Popolazione e turismo"               },
                    { "CheatMod.OPTION[MoveIn]",            "Aumenta tasso di trasferimento"      },
                    { "CheatMod.LABEL[MoveInMult]",         "Moltiplicatore trasferimento"        },
                    { "CheatMod.OPTION[Tourists]",          "Aumenta turisti"                     },
                    { "CheatMod.LABEL[TouristMult]",        "Moltiplicatore turisti"              },
                    { "CheatMod.OPTION[MaxHappiness]",      "Felicit\u00e0 massima"               },
                    { "CheatMod.OPTION[RichCitizens]",      "Cittadini ricchi"                    },
                    { "CheatMod.SECTION[Buildings]",        "Edifici"                             },
                    { "CheatMod.ACTION[UpgradeAll]",        "Tutto a livello 5"                   },
                    { "CheatMod.OPTION[MaxEfficiency]",     "Efficienza max aziende"              },
                    { "CheatMod.ACTION[FillStorage]",       "Riempi magazzino"                    },
                    { "CheatMod.OPTION[KeepStorageFull]",   "Mantieni magazzino sempre pieno"     },
                    { "CheatMod.OPTION[ResetOnNewMap]",     "Resetta trucchi su nuova mappa"      },
                    { "CheatMod.INFO[InstantBuild]",        "Costruzione istantanea attiva \u2014 edifici completati immediatamente"                },
                    { "CheatMod.INFO[Happiness]",           "Tutti i cittadini hanno felicit\u00e0 massima (200)"                                   },
                    { "CheatMod.INFO[RichCitizens]",        "Tutte le famiglie ricevono 500.000 (ogni 5 minuti di gioco)"                           },
                    { "CheatMod.INFO[Efficiency]",          "Efficienza delle aziende massimizzata"                                                 },
                    { "CheatMod.INFO[ResetOnNewMap]",       "Tutti i trucchi saranno disattivati al prossimo caricamento"                            },
                    { "CheatMod.INFO[ForceBuild]",          "Costruzione forzata attiva \u2014 Domanda illimitata"                                  },
                    { "CheatMod.INFO[UpgradeLoading]",      "Caricamento\u2026"                                                                     },
                    { "CheatMod.HINT[Drag]",                "Trascina titolo \u00b7 angolo per ridimensionare"                                      },
                };
            public void Unload() { }
        }

        public class LocaleJA : IDictionarySource
        {
            private readonly Setting _s;
            public LocaleJA(Setting s) => _s = s;
            public IEnumerable<KeyValuePair<string, string>> ReadEntries(IList<IDictionaryEntryError> e, Dictionary<string, int> i) =>
                new Dictionary<string, string>
                {
                    { _s.GetSettingsLocaleID(),                             "CityForge"                         },
                    { _s.GetOptionTabLocaleID(kSection),                    "\u8a2d\u5b9a"                     },
                    { _s.GetOptionGroupLocaleID(kMainGroup),                "\u4e00\u822c"                     },
                    { _s.GetOptionLabelLocaleID(nameof(ShowToolbarButton)), "\u30c4\u30fc\u30eb\u30d0\u30fc\u30dc\u30bf\u30f3\u3092\u8868\u793a" },
                    { _s.GetOptionDescLocaleID(nameof(ShowToolbarButton)),  "\u30c4\u30fc\u30eb\u30d0\u30fc\u306bCityForge\u30dc\u30bf\u30f3\u3092\u8868\u793a\u3057\u307e\u3059\u3002" },
                    { "CheatMod.SECTION[Money]",            "\u8cc7\u91d1"                     },
                    { "CheatMod.OPTION[InfiniteMoney]",     "\u7121\u9650\u306e\u8cc7\u91d1"   },
                    { "CheatMod.SECTION[Milestones]",       "\u30de\u30a4\u30eb\u30b9\u30c8\u30fc\u30f3" },
                    { "CheatMod.ACTION[UnlockAll]",         "\u3059\u3079\u3066\u89e3\u9664"   },
                    { "CheatMod.ACTION[AdvanceToTarget]",   "\u76ee\u6a19\u307e\u3067\u9032\u3081\u308b" },
                    { "CheatMod.LABEL[TargetMilestone]",    "\u76ee\u6a19"                     },
                    { "CheatMod.OPTION[KeepUnlocked]",      "\u6c38\u4e45\u306b\u89e3\u9664\u72b6\u614b\u3092\u7dad\u6301" },
                    { "CheatMod.SECTION[DevTree]",          "\u958b\u767a\u30c4\u30ea\u30fc"   },
                    { "CheatMod.ACTION[AddPoints]",         "+ 228 \u30dd\u30a4\u30f3\u30c8"   },
                    { "CheatMod.SECTION[Demand]",           "\u9700\u8981"                     },
                    { "CheatMod.DEMAND[Residential]",       "\u4f4f\u5b85"                     },
                    { "CheatMod.DEMAND[Commercial]",        "\u5546\u696d"                     },
                    { "CheatMod.DEMAND[Industrial]",        "\u5de5\u696d"                     },
                    { "CheatMod.DEMAND[Office]",            "\u30aa\u30d5\u30a3\u30b9"         },
                    { "CheatMod.LEVEL[Low]",                "\u4f4e"                           },
                    { "CheatMod.LEVEL[Medium]",             "\u4e2d"                           },
                    { "CheatMod.LEVEL[High]",               "\u9ad8"                           },
                    { "CheatMod.ACTION[ForceBuild]",        "\u5f37\u5236\u5efa\u8a2d"         },
                    { "CheatMod.SECTION[Construction]",     "\u5efa\u8a2d\u6642\u9593"         },
                    { "CheatMod.OPTION[InstantBuild]",      "\u5373\u6642\u5efa\u8a2d"         },
                    { "CheatMod.LABEL[BuildSpeed]",         "\u5efa\u8a2d\u901f\u5ea6"         },
                    { "CheatMod.SECTION[Population]",       "\u4eba\u53e3\u3068\u89b3\u5149"   },
                    { "CheatMod.OPTION[MoveIn]",            "\u8ee2\u5165\u7387\u3092\u4e0a\u3052\u308b" },
                    { "CheatMod.LABEL[MoveInMult]",         "\u8ee2\u5165\u500d\u7387"         },
                    { "CheatMod.OPTION[Tourists]",          "\u89b3\u5149\u5ba2\u3092\u5897\u3084\u3059" },
                    { "CheatMod.LABEL[TouristMult]",        "\u89b3\u5149\u5ba2\u500d\u7387"   },
                    { "CheatMod.OPTION[MaxHappiness]",      "\u5e02\u6c11\u306e\u5e78\u798f\u5ea6\u6700\u5927" },
                    { "CheatMod.OPTION[RichCitizens]",      "\u88d5\u798f\u306a\u5e02\u6c11"   },
                    { "CheatMod.SECTION[Buildings]",        "\u5efa\u7269"                     },
                    { "CheatMod.ACTION[UpgradeAll]",        "\u3059\u3079\u3066\u30ec\u30d9\u30eb5\u306b\u30a2\u30c3\u30d7\u30b0\u30ec\u30fc\u30c9" },
                    { "CheatMod.OPTION[MaxEfficiency]",     "\u4f01\u696d\u52b9\u7387\u6700\u5927" },
                    { "CheatMod.ACTION[FillStorage]",       "\u5009\u5eab\u3092\u6e80\u305f\u3059" },
                    { "CheatMod.OPTION[KeepStorageFull]",   "\u5009\u5eab\u3092\u5e38\u306b\u6e80\u676f\u306b\u7dad\u6301" },
                    { "CheatMod.OPTION[ResetOnNewMap]",     "\u65b0\u3057\u3044\u30de\u30c3\u30d7\u3067\u30c1\u30fc\u30c8\u3092\u30ea\u30bb\u30c3\u30c8" },
                    { "CheatMod.INFO[InstantBuild]",        "\u5373\u6642\u5efa\u8a2d\u6709\u52b9 \u2014 \u5efa\u7269\u306f\u5373\u5ea7\u306b\u5b8c\u6210\u3057\u307e\u3059" },
                    { "CheatMod.INFO[Happiness]",           "\u5168\u5e02\u6c11\u304c\u6700\u5927\u306e\u5e78\u798f\u5ea6\u3067\u3059 (200)"                                },
                    { "CheatMod.INFO[RichCitizens]",        "\u5168\u4e16\u5e2f\u306b500,000\u306e\u8cc7\u91d1\u304c\u652f\u7d66\u3055\u308c\u307e\u3059 (\u30b2\u30fc\u30e0\u52055\u5206\u3054\u3068)" },
                    { "CheatMod.INFO[Efficiency]",          "\u4f01\u696d\u52b9\u7387\u304c\u6700\u5927\u5316\u3055\u308c\u3066\u3044\u307e\u3059"                          },
                    { "CheatMod.INFO[ResetOnNewMap]",       "\u6b21\u306e\u30de\u30c3\u30d7\u30ed\u30fc\u30c9\u6642\u306b\u3059\u3079\u3066\u306e\u30c1\u30fc\u30c8\u304c\u7121\u52b9\u306b\u306a\u308a\u307e\u3059" },
                    { "CheatMod.INFO[ForceBuild]",          "\u5f37\u5236\u5efa\u8a2d\u6709\u52b9 \u2014 \u7121\u5236\u9650\u306e\u9700\u8981"                             },
                    { "CheatMod.INFO[UpgradeLoading]",      "\u8aad\u307f\u8fbc\u307f\u4e2d\u2026"                                                                          },
                    { "CheatMod.HINT[Drag]",                "\u30bf\u30a4\u30c8\u30eb\u3092\u30c9\u30e9\u30c3\u30b0 \u00b7 \u89d2\u3067\u30ea\u30b5\u30a4\u30ba"           },
                };
            public void Unload() { }
        }

        public class LocaleKO : IDictionarySource
        {
            private readonly Setting _s;
            public LocaleKO(Setting s) => _s = s;
            public IEnumerable<KeyValuePair<string, string>> ReadEntries(IList<IDictionaryEntryError> e, Dictionary<string, int> i) =>
                new Dictionary<string, string>
                {
                    { _s.GetSettingsLocaleID(),                             "CityForge"                         },
                    { _s.GetOptionTabLocaleID(kSection),                    "\uc124\uc815"                     },
                    { _s.GetOptionGroupLocaleID(kMainGroup),                "\uc77c\ubc18"                     },
                    { _s.GetOptionLabelLocaleID(nameof(ShowToolbarButton)), "\ud234\ubc14 \ubc84\ud2bc \ud45c\uc2dc" },
                    { _s.GetOptionDescLocaleID(nameof(ShowToolbarButton)),  "\ud234\ubc14\uc5d0 CityForge \ubc84\ud2bc\uc744 \ud45c\uc2dc\ud569\ub2c8\ub2e4." },
                    { "CheatMod.SECTION[Money]",            "\uc790\uae08"                     },
                    { "CheatMod.OPTION[InfiniteMoney]",     "\ubb34\ud55c \uc790\uae08"        },
                    { "CheatMod.SECTION[Milestones]",       "\ub9c8\uc77c\uc2a4\ud1a4"         },
                    { "CheatMod.ACTION[UnlockAll]",         "\uc804\uccb4 \uc7a0\uae08 \ud574\uc81c" },
                    { "CheatMod.ACTION[AdvanceToTarget]",   "\ubaa9\ud45c\uae4c\uc9c0 \uc9c4\ud589" },
                    { "CheatMod.LABEL[TargetMilestone]",    "\ubaa9\ud45c"                     },
                    { "CheatMod.OPTION[KeepUnlocked]",      "\uc601\uad6c \uc7a0\uae08 \ud574\uc81c \uc720\uc9c0" },
                    { "CheatMod.SECTION[DevTree]",          "\uac1c\ubc1c \ud2b8\ub9ac"        },
                    { "CheatMod.ACTION[AddPoints]",         "+ 228 \ud3ec\uc778\ud2b8"         },
                    { "CheatMod.SECTION[Demand]",           "\uc218\uc694"                     },
                    { "CheatMod.DEMAND[Residential]",       "\uc8fc\uac70"                     },
                    { "CheatMod.DEMAND[Commercial]",        "\uc0c1\uc5c5"                     },
                    { "CheatMod.DEMAND[Industrial]",        "\uc0b0\uc5c5"                     },
                    { "CheatMod.DEMAND[Office]",            "\uc0ac\ubb34\uc2e4"               },
                    { "CheatMod.LEVEL[Low]",                "\ub0ae\uc74c"                     },
                    { "CheatMod.LEVEL[Medium]",             "\uc911\uac04"                     },
                    { "CheatMod.LEVEL[High]",               "\ub192\uc74c"                     },
                    { "CheatMod.ACTION[ForceBuild]",        "\uac15\uc81c \uac74\uc124"        },
                    { "CheatMod.SECTION[Construction]",     "\uac74\uc124 \uc2dc\uac04"        },
                    { "CheatMod.OPTION[InstantBuild]",      "\uc989\uc2dc \uac74\uc124"        },
                    { "CheatMod.LABEL[BuildSpeed]",         "\uac74\uc124 \uc18d\ub3c4"        },
                    { "CheatMod.SECTION[Population]",       "\uc778\uad6c \ubc0f \uad00\uad11" },
                    { "CheatMod.OPTION[MoveIn]",            "\uc804\uc785\ub960 \uc99d\uac00"  },
                    { "CheatMod.LABEL[MoveInMult]",         "\uc804\uc785 \ubc30\uc728"        },
                    { "CheatMod.OPTION[Tourists]",          "\uad00\uad11\uac1d \uc99d\uac00"  },
                    { "CheatMod.LABEL[TouristMult]",        "\uad00\uad11\uac1d \ubc30\uc728"  },
                    { "CheatMod.OPTION[MaxHappiness]",      "\uc2dc\ubbfc \ud589\ubcf5\ub3c4 \ucd5c\ub300" },
                    { "CheatMod.OPTION[RichCitizens]",      "\ubd80\uc720\ud55c \uc2dc\ubbfc"  },
                    { "CheatMod.SECTION[Buildings]",        "\uac74\ubb3c"                     },
                    { "CheatMod.ACTION[UpgradeAll]",        "\uc804\uccb4 \ub808\ubca8 5\ub85c \uc5c5\uadf8\ub808\uc774\ub4dc" },
                    { "CheatMod.OPTION[MaxEfficiency]",     "\uae30\uc5c5 \ud6a8\uc728 \ucd5c\ub300" },
                    { "CheatMod.ACTION[FillStorage]",       "\ucc3d\uace0 \ucc44\uc6b0\uae30"  },
                    { "CheatMod.OPTION[KeepStorageFull]",   "\ucc3d\uace0 \ud56d\uc0c1 \uac00\ub4dd \uc720\uc9c0" },
                    { "CheatMod.OPTION[ResetOnNewMap]",     "\uc0c8 \ub9f5\uc5d0\uc11c \uce58\ud2b8 \ucd08\uae30\ud654" },
                    { "CheatMod.INFO[InstantBuild]",        "\uc989\uc2dc \uac74\uc124 \ud65c\uc131\ud654 \u2014 \uac74\ubb3c\uc774 \uc989\uc2dc \uc644\ub8cc\ub429\ub2c8\ub2e4" },
                    { "CheatMod.INFO[Happiness]",           "\ubaa8\ub4e0 \uc2dc\ubbfc\uc774 \ucd5c\ub300 \ud589\ubcf5\ub3c4\uc785\ub2c8\ub2e4 (200)"                      },
                    { "CheatMod.INFO[RichCitizens]",        "\ubaa8\ub4e0 \uac00\uad6c\uc5d0 500,000 \uc790\uae08\uc774 \uc9c0\uae09\ub429\ub2c8\ub2e4 (\uac8c\uc784 \ub0b4 5\ubd84\ub9c8\ub2e4)" },
                    { "CheatMod.INFO[Efficiency]",          "\uae30\uc5c5 \ud6a8\uc728\uc774 \ucd5c\ub300\ud654\ub418\uc5c8\uc2b5\ub2c8\ub2e4"                             },
                    { "CheatMod.INFO[ResetOnNewMap]",       "\ub2e4\uc74c \ub9f5 \ub85c\ub4dc \uc2dc \ubaa8\ub4e0 \uce58\ud2b8\uac00 \ube44\ud65c\uc131\ud654\ub429\ub2c8\ub2e4" },
                    { "CheatMod.INFO[ForceBuild]",          "\uac15\uc81c \uac74\uc124 \ud65c\uc131\ud654 \u2014 \ubb34\uc81c\ud55c \uc218\uc694"                           },
                    { "CheatMod.INFO[UpgradeLoading]",      "\ub85c\ub529 \uc911\u2026"                                                                                     },
                    { "CheatMod.HINT[Drag]",                "\uc81c\ubaa9\uc744 \ub4dc\ub798\uadf8 \u00b7 \ubaa8\uc11c\ub9ac\ub85c \ud06c\uae30 \uc870\uc808"              },
                };
            public void Unload() { }
        }

        public class LocalePL : IDictionarySource
        {
            private readonly Setting _s;
            public LocalePL(Setting s) => _s = s;
            public IEnumerable<KeyValuePair<string, string>> ReadEntries(IList<IDictionaryEntryError> e, Dictionary<string, int> i) =>
                new Dictionary<string, string>
                {
                    { _s.GetSettingsLocaleID(),                             "CityForge"                                       },
                    { _s.GetOptionTabLocaleID(kSection),                    "Ustawienia"                                     },
                    { _s.GetOptionGroupLocaleID(kMainGroup),                "Og\u00f3lne"                                    },
                    { _s.GetOptionLabelLocaleID(nameof(ShowToolbarButton)), "Poka\u017c przycisk paska"                      },
                    { _s.GetOptionDescLocaleID(nameof(ShowToolbarButton)),  "Pokazuje przycisk CityForge na pasku narz\u0119dzi." },
                    { "CheatMod.SECTION[Money]",            "Pieni\u0105dze"                      },
                    { "CheatMod.OPTION[InfiniteMoney]",     "Niesko\u0144czone pieni\u0105dze"    },
                    { "CheatMod.SECTION[Milestones]",       "Kamienie milowe"                     },
                    { "CheatMod.ACTION[UnlockAll]",         "Odblokuj wszystko"                   },
                    { "CheatMod.ACTION[AdvanceToTarget]",   "Awansuj do"                          },
                    { "CheatMod.LABEL[TargetMilestone]",    "Cel"                                 },
                    { "CheatMod.OPTION[KeepUnlocked]",      "Utrzymuj stale odblokowane"          },
                    { "CheatMod.SECTION[DevTree]",          "Drzewo rozwoju"                      },
                    { "CheatMod.ACTION[AddPoints]",         "+ 228 Punkt\u00f3w"                  },
                    { "CheatMod.SECTION[Demand]",           "Popyt"                               },
                    { "CheatMod.DEMAND[Residential]",       "Mieszkaniowy"                        },
                    { "CheatMod.DEMAND[Commercial]",        "Handlowy"                            },
                    { "CheatMod.DEMAND[Industrial]",        "Przemys\u0142owy"                    },
                    { "CheatMod.DEMAND[Office]",            "Biurowy"                             },
                    { "CheatMod.LEVEL[Low]",                "Niski"                               },
                    { "CheatMod.LEVEL[Medium]",             "\u015aredni"                         },
                    { "CheatMod.LEVEL[High]",               "Wysoki"                              },
                    { "CheatMod.ACTION[ForceBuild]",        "Wymu\u015b budow\u0119"              },
                    { "CheatMod.SECTION[Construction]",     "Czas budowy"                         },
                    { "CheatMod.OPTION[InstantBuild]",      "Natychmiastowa budowa"               },
                    { "CheatMod.LABEL[BuildSpeed]",         "Pr\u0119dko\u015b\u0107 budowy"      },
                    { "CheatMod.SECTION[Population]",       "Populacja i turystyka"               },
                    { "CheatMod.OPTION[MoveIn]",            "Zwi\u0119ksz tempo zaludniania"      },
                    { "CheatMod.LABEL[MoveInMult]",         "Mno\u017cnik zaludniania"            },
                    { "CheatMod.OPTION[Tourists]",          "Zwi\u0119ksz turyst\u00f3w"          },
                    { "CheatMod.LABEL[TouristMult]",        "Mno\u017cnik turyst\u00f3w"          },
                    { "CheatMod.OPTION[MaxHappiness]",      "Maks. szcz\u0119\u015bcie obywateli" },
                    { "CheatMod.OPTION[RichCitizens]",      "Bogaci obywatele"                    },
                    { "CheatMod.SECTION[Buildings]",        "Budynki"                             },
                    { "CheatMod.ACTION[UpgradeAll]",        "Ulepsz wszystko do poziomu 5"        },
                    { "CheatMod.OPTION[MaxEfficiency]",     "Maks. wydajno\u015b\u0107 firm"      },
                    { "CheatMod.ACTION[FillStorage]",       "Wype\u0142nij magazyn"               },
                    { "CheatMod.OPTION[KeepStorageFull]",   "Utrzymuj magazyn pe\u0142ny"         },
                    { "CheatMod.OPTION[ResetOnNewMap]",     "Resetuj cheaty na nowej mapie"       },
                    { "CheatMod.INFO[InstantBuild]",        "Natychmiastowa budowa aktywna \u2014 budynki s\u0105 uko\u0144czone od razu"           },
                    { "CheatMod.INFO[Happiness]",           "Wszyscy obywatele maj\u0105 maksymalne szcz\u0119\u015bcie (200)"                      },
                    { "CheatMod.INFO[RichCitizens]",        "Wszystkie gospodarstwa otrzymuj\u0105 500 000 (co 5 minut gry)"                        },
                    { "CheatMod.INFO[Efficiency]",          "Wydajno\u015b\u0107 firm zmaksymalizowana"                                             },
                    { "CheatMod.INFO[ResetOnNewMap]",       "Wszystkie cheaty zostan\u0105 wy\u0142\u0105czone przy nast\u0119pnym \u0142adowaniu mapy" },
                    { "CheatMod.INFO[ForceBuild]",          "Wymuszenie budowy aktywne \u2014 Nieograniczony popyt"                                 },
                    { "CheatMod.INFO[UpgradeLoading]",      "\u0141adowanie\u2026"                                                                   },
                    { "CheatMod.HINT[Drag]",                "Przeci\u0105gnij tytu\u0142 \u00b7 r\u00f3g aby zmieni\u0107 rozmiar"                  },
                };
            public void Unload() { }
        }

        public class LocalePT : IDictionarySource
        {
            private readonly Setting _s;
            public LocalePT(Setting s) => _s = s;
            public IEnumerable<KeyValuePair<string, string>> ReadEntries(IList<IDictionaryEntryError> e, Dictionary<string, int> i) =>
                new Dictionary<string, string>
                {
                    { _s.GetSettingsLocaleID(),                             "CityForge"                                        },
                    { _s.GetOptionTabLocaleID(kSection),                    "Configura\u00e7\u00f5es"                         },
                    { _s.GetOptionGroupLocaleID(kMainGroup),                "Geral"                                           },
                    { _s.GetOptionLabelLocaleID(nameof(ShowToolbarButton)), "Mostrar bot\u00e3o na barra"                     },
                    { _s.GetOptionDescLocaleID(nameof(ShowToolbarButton)),  "Mostra o bot\u00e3o do CityForge na barra."       },
                    { "CheatMod.SECTION[Money]",            "Dinheiro"                            },
                    { "CheatMod.OPTION[InfiniteMoney]",     "Dinheiro infinito"                   },
                    { "CheatMod.SECTION[Milestones]",       "Marcos"                              },
                    { "CheatMod.ACTION[UnlockAll]",         "Desbloquear tudo"                    },
                    { "CheatMod.ACTION[AdvanceToTarget]",   "Avan\u00e7ar at\u00e9"               },
                    { "CheatMod.LABEL[TargetMilestone]",    "Meta"                                },
                    { "CheatMod.OPTION[KeepUnlocked]",      "Manter desbloqueado"                 },
                    { "CheatMod.SECTION[DevTree]",          "\u00c1rvore de desenvolvimento"      },
                    { "CheatMod.ACTION[AddPoints]",         "+ 228 Pontos"                        },
                    { "CheatMod.SECTION[Demand]",           "Demanda"                             },
                    { "CheatMod.DEMAND[Residential]",       "Residencial"                         },
                    { "CheatMod.DEMAND[Commercial]",        "Comercial"                           },
                    { "CheatMod.DEMAND[Industrial]",        "Industrial"                          },
                    { "CheatMod.DEMAND[Office]",            "Escrit\u00f3rio"                     },
                    { "CheatMod.LEVEL[Low]",                "Baixo"                               },
                    { "CheatMod.LEVEL[Medium]",             "M\u00e9dio"                          },
                    { "CheatMod.LEVEL[High]",               "Alto"                                },
                    { "CheatMod.ACTION[ForceBuild]",        "Constru\u00e7\u00e3o for\u00e7ada"  },
                    { "CheatMod.SECTION[Construction]",     "Tempo de constru\u00e7\u00e3o"      },
                    { "CheatMod.OPTION[InstantBuild]",      "Constru\u00e7\u00e3o instant\u00e2nea" },
                    { "CheatMod.LABEL[BuildSpeed]",         "Velocidade de constru\u00e7\u00e3o" },
                    { "CheatMod.SECTION[Population]",       "Popula\u00e7\u00e3o e Turismo"      },
                    { "CheatMod.OPTION[MoveIn]",            "Aumentar taxa de mudan\u00e7a"      },
                    { "CheatMod.LABEL[MoveInMult]",         "Multiplicador de mudan\u00e7a"      },
                    { "CheatMod.OPTION[Tourists]",          "Aumentar turistas"                   },
                    { "CheatMod.LABEL[TouristMult]",        "Multiplicador de turistas"           },
                    { "CheatMod.OPTION[MaxHappiness]",      "Felicidade m\u00e1xima cidad\u00e3os" },
                    { "CheatMod.OPTION[RichCitizens]",      "Cidad\u00e3os ricos"                },
                    { "CheatMod.SECTION[Buildings]",        "Edif\u00edcios"                     },
                    { "CheatMod.ACTION[UpgradeAll]",        "Melhorar tudo para n\u00edvel 5"    },
                    { "CheatMod.OPTION[MaxEfficiency]",     "Efici\u00eancia m\u00e1x empresas"  },
                    { "CheatMod.ACTION[FillStorage]",       "Encher armaz\u00e9m"                },
                    { "CheatMod.OPTION[KeepStorageFull]",   "Manter armaz\u00e9m sempre cheio"   },
                    { "CheatMod.OPTION[ResetOnNewMap]",     "Resetar trapa\u00e7as em novo mapa" },
                    { "CheatMod.INFO[InstantBuild]",        "Constru\u00e7\u00e3o instant\u00e2nea ativa \u2014 edif\u00edcios conclu\u00eddos imediatamente" },
                    { "CheatMod.INFO[Happiness]",           "Todos os cidad\u00e3os t\u00eam felicidade m\u00e1xima (200)"                                   },
                    { "CheatMod.INFO[RichCitizens]",        "Todos os lares recebem 500.000 (a cada 5 minutos de jogo)"                                       },
                    { "CheatMod.INFO[Efficiency]",          "Efici\u00eancia das empresas maximizada"                                                         },
                    { "CheatMod.INFO[ResetOnNewMap]",       "Todas as trapa\u00e7as ser\u00e3o desativadas no pr\u00f3ximo carregamento de mapa"              },
                    { "CheatMod.INFO[ForceBuild]",          "Constru\u00e7\u00e3o for\u00e7ada ativa \u2014 Demanda ilimitada"                               },
                    { "CheatMod.INFO[UpgradeLoading]",      "Carregando\u2026"                                                                                },
                    { "CheatMod.HINT[Drag]",                "Arraste t\u00edtulo \u00b7 canto para redimensionar"                                             },
                };
            public void Unload() { }
        }

        public class LocaleRU : IDictionarySource
        {
            private readonly Setting _s;
            public LocaleRU(Setting s) => _s = s;
            public IEnumerable<KeyValuePair<string, string>> ReadEntries(IList<IDictionaryEntryError> e, Dictionary<string, int> i) =>
                new Dictionary<string, string>
                {
                    { _s.GetSettingsLocaleID(),                             "CityForge"                                  },
                    { _s.GetOptionTabLocaleID(kSection),                    "\u041d\u0430\u0441\u0442\u0440\u043e\u0439\u043a\u0438" },
                    { _s.GetOptionGroupLocaleID(kMainGroup),                "\u041e\u0431\u0449\u0438\u0435"             },
                    { _s.GetOptionLabelLocaleID(nameof(ShowToolbarButton)), "\u041f\u043e\u043a\u0430\u0437\u0430\u0442\u044c \u043a\u043d\u043e\u043f\u043a\u0443 \u043f\u0430\u043d\u0435\u043b\u0438" },
                    { _s.GetOptionDescLocaleID(nameof(ShowToolbarButton)),  "\u041f\u043e\u043a\u0430\u0437\u044b\u0432\u0430\u0435\u0442 \u043a\u043d\u043e\u043f\u043a\u0443 CityForge \u043d\u0430 \u043f\u0430\u043d\u0435\u043b\u0438." },
                    { "CheatMod.SECTION[Money]",            "\u0414\u0435\u043d\u044c\u0433\u0438" },
                    { "CheatMod.OPTION[InfiniteMoney]",     "\u0411\u0435\u0441\u043a\u043e\u043d\u0435\u0447\u043d\u044b\u0435 \u0434\u0435\u043d\u044c\u0433\u0438" },
                    { "CheatMod.SECTION[Milestones]",       "\u0412\u0435\u0445\u0438"             },
                    { "CheatMod.ACTION[UnlockAll]",         "\u0420\u0430\u0437\u0431\u043b\u043e\u043a\u0438\u0440\u043e\u0432\u0430\u0442\u044c \u0432\u0441\u0451" },
                    { "CheatMod.ACTION[AdvanceToTarget]",   "\u041f\u0440\u043e\u0434\u0432\u0438\u043d\u0443\u0442\u044c \u0434\u043e" },
                    { "CheatMod.LABEL[TargetMilestone]",    "\u0426\u0435\u043b\u044c"             },
                    { "CheatMod.OPTION[KeepUnlocked]",      "\u0414\u0435\u0440\u0436\u0430\u0442\u044c \u0440\u0430\u0437\u0431\u043b\u043e\u043a\u0438\u0440\u043e\u0432\u0430\u043d\u043d\u044b\u043c" },
                    { "CheatMod.SECTION[DevTree]",          "\u0414\u0435\u0440\u0435\u0432\u043e \u0440\u0430\u0437\u0432\u0438\u0442\u0438\u044f" },
                    { "CheatMod.ACTION[AddPoints]",         "+ 228 \u041e\u0447\u043a\u043e\u0432" },
                    { "CheatMod.SECTION[Demand]",           "\u0421\u043f\u0440\u043e\u0441"       },
                    { "CheatMod.DEMAND[Residential]",       "\u0416\u0438\u043b\u043e\u0439"       },
                    { "CheatMod.DEMAND[Commercial]",        "\u041a\u043e\u043c\u043c\u0435\u0440\u0447\u0435\u0441\u043a\u0438\u0439" },
                    { "CheatMod.DEMAND[Industrial]",        "\u041f\u0440\u043e\u043c\u044b\u0448\u043b\u0435\u043d\u043d\u044b\u0439" },
                    { "CheatMod.DEMAND[Office]",            "\u041e\u0444\u0438\u0441\u043d\u044b\u0439" },
                    { "CheatMod.LEVEL[Low]",                "\u041d\u0438\u0437\u043a\u0438\u0439" },
                    { "CheatMod.LEVEL[Medium]",             "\u0421\u0440\u0435\u0434\u043d\u0438\u0439" },
                    { "CheatMod.LEVEL[High]",               "\u0412\u044b\u0441\u043e\u043a\u0438\u0439" },
                    { "CheatMod.ACTION[ForceBuild]",        "\u041f\u0440\u0438\u043d\u0443\u0434\u0438\u0442\u0435\u043b\u044c\u043d\u0430\u044f \u0441\u0442\u0440\u043e\u0439\u043a\u0430" },
                    { "CheatMod.SECTION[Construction]",     "\u0412\u0440\u0435\u043c\u044f \u0441\u0442\u0440\u043e\u0438\u0442\u0435\u043b\u044c\u0441\u0442\u0432\u0430" },
                    { "CheatMod.OPTION[InstantBuild]",      "\u041c\u0433\u043d\u043e\u0432\u0435\u043d\u043d\u043e\u0435 \u0441\u0442\u0440\u043e\u0438\u0442\u0435\u043b\u044c\u0441\u0442\u0432\u043e" },
                    { "CheatMod.LABEL[BuildSpeed]",         "\u0421\u043a\u043e\u0440\u043e\u0441\u0442\u044c \u0441\u0442\u0440\u043e\u0438\u0442\u0435\u043b\u044c\u0441\u0442\u0432\u0430" },
                    { "CheatMod.SECTION[Population]",       "\u041d\u0430\u0441\u0435\u043b\u0435\u043d\u0438\u0435 \u0438 \u0442\u0443\u0440\u0438\u0437\u043c" },
                    { "CheatMod.OPTION[MoveIn]",            "\u0423\u0432\u0435\u043b\u0438\u0447\u0438\u0442\u044c \u043f\u0440\u0438\u0442\u043e\u043a \u0436\u0438\u0442\u0435\u043b\u0435\u0439" },
                    { "CheatMod.LABEL[MoveInMult]",         "\u041c\u043d\u043e\u0436\u0438\u0442\u0435\u043b\u044c \u043f\u0440\u0438\u0442\u043e\u043a\u0430" },
                    { "CheatMod.OPTION[Tourists]",          "\u0423\u0432\u0435\u043b\u0438\u0447\u0438\u0442\u044c \u0442\u0443\u0440\u0438\u0441\u0442\u043e\u0432" },
                    { "CheatMod.LABEL[TouristMult]",        "\u041c\u043d\u043e\u0436\u0438\u0442\u0435\u043b\u044c \u0442\u0443\u0440\u0438\u0441\u0442\u043e\u0432" },
                    { "CheatMod.OPTION[MaxHappiness]",      "\u041c\u0430\u043a\u0441. \u0441\u0447\u0430\u0441\u0442\u044c\u0435 \u0433\u0440\u0430\u0436\u0434\u0430\u043d" },
                    { "CheatMod.OPTION[RichCitizens]",      "\u0411\u043e\u0433\u0430\u0442\u044b\u0435 \u0433\u0440\u0430\u0436\u0434\u0430\u043d\u0435" },
                    { "CheatMod.SECTION[Buildings]",        "\u0417\u0434\u0430\u043d\u0438\u044f" },
                    { "CheatMod.ACTION[UpgradeAll]",        "\u0423\u043b\u0443\u0447\u0448\u0438\u0442\u044c \u0432\u0441\u0451 \u0434\u043e \u0443\u0440\u043e\u0432\u043d\u044f 5" },
                    { "CheatMod.OPTION[MaxEfficiency]",     "\u041c\u0430\u043a\u0441. \u044d\u0444\u0444\u0435\u043a\u0442\u0438\u0432\u043d\u043e\u0441\u0442\u044c \u043a\u043e\u043c\u043f\u0430\u043d\u0438\u0439" },
                    { "CheatMod.ACTION[FillStorage]",       "\u0417\u0430\u043f\u043e\u043b\u043d\u0438\u0442\u044c \u0441\u043a\u043b\u0430\u0434" },
                    { "CheatMod.OPTION[KeepStorageFull]",   "\u0414\u0435\u0440\u0436\u0430\u0442\u044c \u0441\u043a\u043b\u0430\u0434 \u043f\u043e\u0441\u0442\u043e\u044f\u043d\u043d\u043e \u043f\u043e\u043b\u043d\u044b\u043c" },
                    { "CheatMod.OPTION[ResetOnNewMap]",     "\u0421\u0431\u0440\u043e\u0441\u0438\u0442\u044c \u0447\u0438\u0442\u044b \u043d\u0430 \u043d\u043e\u0432\u043e\u0439 \u043a\u0430\u0440\u0442\u0435" },
                    { "CheatMod.INFO[InstantBuild]",        "\u041c\u0433\u043d\u043e\u0432\u0435\u043d\u043d\u043e\u0435 \u0441\u0442\u0440\u043e\u0438\u0442\u0435\u043b\u044c\u0441\u0442\u0432\u043e \u0430\u043a\u0442\u0438\u0432\u043d\u043e \u2014 \u0437\u0434\u0430\u043d\u0438\u044f \u0437\u0430\u0432\u0435\u0440\u0448\u0430\u044e\u0442\u0441\u044f \u043c\u0433\u043d\u043e\u0432\u0435\u043d\u043d\u043e" },
                    { "CheatMod.INFO[Happiness]",           "\u0412\u0441\u0435 \u0433\u0440\u0430\u0436\u0434\u0430\u043d\u0435 \u0438\u043c\u0435\u044e\u0442 \u043c\u0430\u043a\u0441\u0438\u043c\u0430\u043b\u044c\u043d\u043e\u0435 \u0441\u0447\u0430\u0441\u0442\u044c\u0435 (200)" },
                    { "CheatMod.INFO[RichCitizens]",        "\u0412\u0441\u0435 \u0434\u043e\u043c\u043e\u0445\u043e\u0437\u044f\u0439\u0441\u0442\u0432\u0430 \u043f\u043e\u043b\u0443\u0447\u0430\u044e\u0442 500 000 (\u043a\u0430\u0436\u0434\u044b\u0435 5 \u0438\u0433\u0440\u043e\u0432\u044b\u0445 \u043c\u0438\u043d\u0443\u0442)" },
                    { "CheatMod.INFO[Efficiency]",          "\u042d\u0444\u0444\u0435\u043a\u0442\u0438\u0432\u043d\u043e\u0441\u0442\u044c \u043a\u043e\u043c\u043f\u0430\u043d\u0438\u0439 \u043c\u0430\u043a\u0441\u0438\u043c\u0430\u043b\u044c\u043d\u0430" },
                    { "CheatMod.INFO[ResetOnNewMap]",       "\u0412\u0441\u0435 \u0447\u0438\u0442\u044b \u0431\u0443\u0434\u0443\u0442 \u043e\u0442\u043a\u043b\u044e\u0447\u0435\u043d\u044b \u043f\u0440\u0438 \u0441\u043b\u0435\u0434\u0443\u044e\u0449\u0435\u0439 \u0437\u0430\u0433\u0440\u0443\u0437\u043a\u0435 \u043a\u0430\u0440\u0442\u044b" },
                    { "CheatMod.INFO[ForceBuild]",          "\u041f\u0440\u0438\u043d\u0443\u0434\u0438\u0442\u0435\u043b\u044c\u043d\u0430\u044f \u0441\u0442\u0440\u043e\u0439\u043a\u0430 \u0430\u043a\u0442\u0438\u0432\u043d\u0430 \u2014 \u041d\u0435\u043e\u0433\u0440\u0430\u043d\u0438\u0447\u0435\u043d\u043d\u044b\u0439 \u0441\u043f\u0440\u043e\u0441" },
                    { "CheatMod.INFO[UpgradeLoading]",      "\u0417\u0430\u0433\u0440\u0443\u0437\u043a\u0430\u2026"                                            },
                    { "CheatMod.HINT[Drag]",                "\u041f\u0435\u0440\u0435\u0442\u0430\u0449\u0438\u0442\u0435 \u0437\u0430\u0433\u043e\u043b\u043e\u0432\u043e\u043a \u00b7 \u0443\u0433\u043e\u043b \u0434\u043b\u044f \u0438\u0437\u043c\u0435\u043d\u0435\u043d\u0438\u044f \u0440\u0430\u0437\u043c\u0435\u0440\u0430" },
                };
            public void Unload() { }
        }

        public class LocaleZHHans : IDictionarySource
        {
            private readonly Setting _s;
            public LocaleZHHans(Setting s) => _s = s;
            public IEnumerable<KeyValuePair<string, string>> ReadEntries(IList<IDictionaryEntryError> e, Dictionary<string, int> i) =>
                new Dictionary<string, string>
                {
                    { _s.GetSettingsLocaleID(),                             "CityForge"                  },
                    { _s.GetOptionTabLocaleID(kSection),                    "\u8bbe\u7f6e"               },
                    { _s.GetOptionGroupLocaleID(kMainGroup),                "\u5e38\u89c4"               },
                    { _s.GetOptionLabelLocaleID(nameof(ShowToolbarButton)), "\u663e\u793a\u5de5\u5177\u680f\u6309\u94ae" },
                    { _s.GetOptionDescLocaleID(nameof(ShowToolbarButton)),  "\u5728\u5de5\u5177\u680f\u4e2d\u663e\u793aCityForge\u6309\u94ae\u3002" },
                    { "CheatMod.SECTION[Money]",            "\u8d44\u91d1"                     },
                    { "CheatMod.OPTION[InfiniteMoney]",     "\u65e0\u9650\u8d44\u91d1"         },
                    { "CheatMod.SECTION[Milestones]",       "\u91cc\u7a0b\u7891"               },
                    { "CheatMod.ACTION[UnlockAll]",         "\u5168\u90e8\u89e3\u9501"         },
                    { "CheatMod.ACTION[AdvanceToTarget]",   "\u63a8\u8fdb\u5230"               },
                    { "CheatMod.LABEL[TargetMilestone]",    "\u76ee\u6807"                     },
                    { "CheatMod.OPTION[KeepUnlocked]",      "\u6c38\u4e45\u4fdd\u6301\u89e3\u9501" },
                    { "CheatMod.SECTION[DevTree]",          "\u53d1\u5c55\u6811"               },
                    { "CheatMod.ACTION[AddPoints]",         "+ 228 \u70b9\u6570"               },
                    { "CheatMod.SECTION[Demand]",           "\u9700\u6c42"                     },
                    { "CheatMod.DEMAND[Residential]",       "\u4f4f\u5b85"                     },
                    { "CheatMod.DEMAND[Commercial]",        "\u5546\u4e1a"                     },
                    { "CheatMod.DEMAND[Industrial]",        "\u5de5\u4e1a"                     },
                    { "CheatMod.DEMAND[Office]",            "\u529e\u516c"                     },
                    { "CheatMod.LEVEL[Low]",                "\u4f4e"                           },
                    { "CheatMod.LEVEL[Medium]",             "\u4e2d"                           },
                    { "CheatMod.LEVEL[High]",               "\u9ad8"                           },
                    { "CheatMod.ACTION[ForceBuild]",        "\u5f3a\u5236\u5efa\u9020"         },
                    { "CheatMod.SECTION[Construction]",     "\u5efa\u9020\u65f6\u95f4"         },
                    { "CheatMod.OPTION[InstantBuild]",      "\u5373\u65f6\u5efa\u9020"         },
                    { "CheatMod.LABEL[BuildSpeed]",         "\u5efa\u9020\u901f\u5ea6"         },
                    { "CheatMod.SECTION[Population]",       "\u4eba\u53e3\u4e0e\u65c5\u6e38"   },
                    { "CheatMod.OPTION[MoveIn]",            "\u63d0\u9ad8\u8fc1\u5165\u7387"   },
                    { "CheatMod.LABEL[MoveInMult]",         "\u8fc1\u5165\u500d\u7387"         },
                    { "CheatMod.OPTION[Tourists]",          "\u589e\u52a0\u6e38\u5ba2"         },
                    { "CheatMod.LABEL[TouristMult]",        "\u6e38\u5ba2\u500d\u7387"         },
                    { "CheatMod.OPTION[MaxHappiness]",      "\u5e02\u6c11\u5e78\u798f\u5ea6\u6700\u5927" },
                    { "CheatMod.OPTION[RichCitizens]",      "\u5bcc\u88d5\u5e02\u6c11"         },
                    { "CheatMod.SECTION[Buildings]",        "\u5efa\u7b51"                     },
                    { "CheatMod.ACTION[UpgradeAll]",        "\u5168\u90e8\u5347\u7ea7\u52305\u7ea7" },
                    { "CheatMod.OPTION[MaxEfficiency]",     "\u4f01\u4e1a\u6548\u7387\u6700\u5927" },
                    { "CheatMod.ACTION[FillStorage]",       "\u586b\u6ee1\u4ed3\u5e93"         },
                    { "CheatMod.OPTION[KeepStorageFull]",   "\u4fdd\u6301\u4ed3\u5e93\u59cb\u7ec8\u6ee1\u8f7d" },
                    { "CheatMod.OPTION[ResetOnNewMap]",     "\u65b0\u5730\u56fe\u65f6\u91cd\u7f6e\u4f5c\u5f0a" },
                    { "CheatMod.INFO[InstantBuild]",        "\u5373\u65f6\u5efa\u9020\u5df2\u542f\u7528 \u2014 \u5efa\u7b51\u7acb\u5373\u5b8c\u6210" },
                    { "CheatMod.INFO[Happiness]",           "\u6240\u6709\u5e02\u6c11\u62e5\u6709\u6700\u5927\u5e78\u798f\u5ea6 (200)"               },
                    { "CheatMod.INFO[RichCitizens]",        "\u6240\u6709\u5bb6\u5ead\u83b7\u5f97500,000\u8d44\u91d1 (\u6bcf\u6e38\u620f5\u5206\u949f)" },
                    { "CheatMod.INFO[Efficiency]",          "\u4f01\u4e1a\u6548\u7387\u5df2\u6700\u5927\u5316"                                       },
                    { "CheatMod.INFO[ResetOnNewMap]",       "\u4e0b\u6b21\u52a0\u8f7d\u5730\u56fe\u65f6\u6240\u6709\u4f5c\u5f0a\u5c06\u88ab\u7981\u7528" },
                    { "CheatMod.INFO[ForceBuild]",          "\u5f3a\u5236\u5efa\u9020\u5df2\u542f\u7528 \u2014 \u65e0\u9650\u5236\u9700\u6c42"     },
                    { "CheatMod.INFO[UpgradeLoading]",      "\u52a0\u8f7d\u4e2d\u2026"                                                               },
                    { "CheatMod.HINT[Drag]",                "\u62d6\u52a8\u6807\u9898 \u00b7 \u89d2\u843d\u8c03\u6574\u5927\u5c0f"                  },
                };
            public void Unload() { }
        }

        public class LocaleZHHant : IDictionarySource
        {
            private readonly Setting _s;
            public LocaleZHHant(Setting s) => _s = s;
            public IEnumerable<KeyValuePair<string, string>> ReadEntries(IList<IDictionaryEntryError> e, Dictionary<string, int> i) =>
                new Dictionary<string, string>
                {
                    { _s.GetSettingsLocaleID(),                             "CityForge"                  },
                    { _s.GetOptionTabLocaleID(kSection),                    "\u8a2d\u5b9a"               },
                    { _s.GetOptionGroupLocaleID(kMainGroup),                "\u4e00\u822c"               },
                    { _s.GetOptionLabelLocaleID(nameof(ShowToolbarButton)), "\u986f\u793a\u5de5\u5177\u5217\u6309\u9215" },
                    { _s.GetOptionDescLocaleID(nameof(ShowToolbarButton)),  "\u5728\u5de5\u5177\u5217\u4e2d\u986f\u793aCityForge\u6309\u9215\u3002" },
                    { "CheatMod.SECTION[Money]",            "\u8cc7\u91d1"                     },
                    { "CheatMod.OPTION[InfiniteMoney]",     "\u7121\u9650\u8cc7\u91d1"         },
                    { "CheatMod.SECTION[Milestones]",       "\u91cc\u7a0b\u7891"               },
                    { "CheatMod.ACTION[UnlockAll]",         "\u5168\u90e8\u89e3\u9396"         },
                    { "CheatMod.ACTION[AdvanceToTarget]",   "\u63a8\u9032\u5230"               },
                    { "CheatMod.LABEL[TargetMilestone]",    "\u76ee\u6a19"                     },
                    { "CheatMod.OPTION[KeepUnlocked]",      "\u6c38\u4e45\u4fdd\u6301\u89e3\u9396" },
                    { "CheatMod.SECTION[DevTree]",          "\u767c\u5c55\u6a39"               },
                    { "CheatMod.ACTION[AddPoints]",         "+ 228 \u9ede\u6578"               },
                    { "CheatMod.SECTION[Demand]",           "\u9700\u6c42"                     },
                    { "CheatMod.DEMAND[Residential]",       "\u4f4f\u5b85"                     },
                    { "CheatMod.DEMAND[Commercial]",        "\u5546\u696d"                     },
                    { "CheatMod.DEMAND[Industrial]",        "\u5de5\u696d"                     },
                    { "CheatMod.DEMAND[Office]",            "\u8fa6\u516c"                     },
                    { "CheatMod.LEVEL[Low]",                "\u4f4e"                           },
                    { "CheatMod.LEVEL[Medium]",             "\u4e2d"                           },
                    { "CheatMod.LEVEL[High]",               "\u9ad8"                           },
                    { "CheatMod.ACTION[ForceBuild]",        "\u5f37\u5236\u5efa\u9020"         },
                    { "CheatMod.SECTION[Construction]",     "\u5efa\u9020\u6642\u9593"         },
                    { "CheatMod.OPTION[InstantBuild]",      "\u5373\u6642\u5efa\u9020"         },
                    { "CheatMod.LABEL[BuildSpeed]",         "\u5efa\u9020\u901f\u5ea6"         },
                    { "CheatMod.SECTION[Population]",       "\u4eba\u53e3\u8207\u89c0\u5149"   },
                    { "CheatMod.OPTION[MoveIn]",            "\u63d0\u9ad8\u9077\u5165\u7387"   },
                    { "CheatMod.LABEL[MoveInMult]",         "\u9077\u5165\u500d\u7387"         },
                    { "CheatMod.OPTION[Tourists]",          "\u589e\u52a0\u904a\u5ba2"         },
                    { "CheatMod.LABEL[TouristMult]",        "\u904a\u5ba2\u500d\u7387"         },
                    { "CheatMod.OPTION[MaxHappiness]",      "\u5e02\u6c11\u5e78\u798f\u5ea6\u6700\u5927" },
                    { "CheatMod.OPTION[RichCitizens]",      "\u5bcc\u88d5\u5e02\u6c11"         },
                    { "CheatMod.SECTION[Buildings]",        "\u5efa\u7bc9"                     },
                    { "CheatMod.ACTION[UpgradeAll]",        "\u5168\u90e8\u5347\u7d1a\u52305\u7d1a" },
                    { "CheatMod.OPTION[MaxEfficiency]",     "\u4f01\u696d\u6548\u7387\u6700\u5927" },
                    { "CheatMod.ACTION[FillStorage]",       "\u586b\u6eff\u5009\u5eab"         },
                    { "CheatMod.OPTION[KeepStorageFull]",   "\u4fdd\u6301\u5009\u5eab\u59cb\u7d42\u6eff\u8f09" },
                    { "CheatMod.OPTION[ResetOnNewMap]",     "\u65b0\u5730\u5716\u6642\u91cd\u7f6e\u4f5c\u5f0a" },
                    { "CheatMod.INFO[InstantBuild]",        "\u5373\u6642\u5efa\u9020\u5df2\u555f\u7528 \u2014 \u5efa\u7bc9\u7acb\u5373\u5b8c\u6210" },
                    { "CheatMod.INFO[Happiness]",           "\u6240\u6709\u5e02\u6c11\u64c1\u6709\u6700\u5927\u5e78\u798f\u5ea6 (200)"               },
                    { "CheatMod.INFO[RichCitizens]",        "\u6240\u6709\u5bb6\u5ead\u7372\u5f97500,000\u8cc7\u91d1 (\u6bcf\u904a\u62325\u5206\u9418)" },
                    { "CheatMod.INFO[Efficiency]",          "\u4f01\u696d\u6548\u7387\u5df2\u6700\u5927\u5316"                                       },
                    { "CheatMod.INFO[ResetOnNewMap]",       "\u4e0b\u6b21\u8f09\u5165\u5730\u5716\u6642\u6240\u6709\u4f5c\u5f0a\u5c07\u88ab\u7981\u7528" },
                    { "CheatMod.INFO[ForceBuild]",          "\u5f37\u5236\u5efa\u9020\u5df2\u555f\u7528 \u2014 \u7121\u9650\u5236\u9700\u6c42"     },
                    { "CheatMod.INFO[UpgradeLoading]",      "\u8f09\u5165\u4e2d\u2026"                                                               },
                    { "CheatMod.HINT[Drag]",                "\u62d6\u52d5\u6a19\u984c \u00b7 \u89d2\u843d\u8abf\u6574\u5927\u5c0f"                  },
                };
            public void Unload() { }
        }
    }
}