import React, { useState, useCallback, useRef, useEffect } from "react";
import { bindValue, useValue, trigger } from "cs2/api";
import { ModRegistrar }                 from "cs2/modding";
import { FloatingButton, Tooltip }      from "cs2/ui";
import { useLocalization }              from "cs2/l10n";
import cityForgeIcon from "./cityforge-icon.svg";

const PanelVisible$          = bindValue<boolean>("cheatmod","PanelVisible",           false);
const ShowButton$            = bindValue<boolean>("cheatmod","ShowButton",             true);
const InfiniteMoney$         = bindValue<boolean>("cheatmod","InfiniteMoney",          false);
const KeepMilestones$        = bindValue<boolean>("cheatmod","KeepMilestones",         false);
const TargetMilestone$       = bindValue<number> ("cheatmod","TargetMilestone",        3);
const OverrideRes$           = bindValue<boolean>("cheatmod","OverrideRes",            false);
const ResLow$                = bindValue<number> ("cheatmod","ResLow",                 50);
const ResMed$                = bindValue<number> ("cheatmod","ResMed",                 50);
const ResHigh$               = bindValue<number> ("cheatmod","ResHigh",                50);
const ForceResBuild$         = bindValue<boolean>("cheatmod","ForceResBuild",          false);
const OverrideCom$           = bindValue<boolean>("cheatmod","OverrideCom",            false);
const ComValue$              = bindValue<number> ("cheatmod","ComValue",               50);
const ForceComBuild$         = bindValue<boolean>("cheatmod","ForceComBuild",          false);
const OverrideInd$           = bindValue<boolean>("cheatmod","OverrideInd",            false);
const IndValue$              = bindValue<number> ("cheatmod","IndValue",               50);
const ForceIndBuild$         = bindValue<boolean>("cheatmod","ForceIndBuild",          false);
const OverrideOff$           = bindValue<boolean>("cheatmod","OverrideOff",            false);
const OffValue$              = bindValue<number> ("cheatmod","OffValue",               50);
const ForceOffBuild$         = bindValue<boolean>("cheatmod","ForceOffBuild",          false);
const InstantConstruction$   = bindValue<boolean>("cheatmod","InstantConstruction",    false);
const ConstructionSpeedIndex$= bindValue<number> ("cheatmod","ConstructionSpeedIndex", 1);
const OverrideMoveIn$        = bindValue<boolean>("cheatmod","OverrideMoveIn",         false);
const MoveInMultiplier$      = bindValue<number> ("cheatmod","MoveInMultiplier",       1);
const OverrideTourists$      = bindValue<boolean>("cheatmod","OverrideTourists",       false);
const TouristMultiplier$     = bindValue<number> ("cheatmod","TouristMultiplier",      1);
const MaxHappiness$          = bindValue<boolean>("cheatmod","MaxHappiness",           false);
const RichCitizens$          = bindValue<boolean>("cheatmod","RichCitizens",           false);
const MaxEducation$          = bindValue<boolean>("cheatmod","MaxEducation",           false);
const MaxCompanyEfficiency$  = bindValue<boolean>("cheatmod","MaxCompanyEfficiency",   false);
const ResetOnNewMap$         = bindValue<boolean>("cheatmod","ResetOnNewMap",          true);
const BuildingLevelAvailable$ = bindValue<boolean>("cheatmod","BuildingLevelAvailable", false);
const KeepStorageFull$        = bindValue<boolean>("cheatmod","KeepStorageFull",         false);
const PanelBgColor$           = bindValue<string> ("cheatmod","PanelBgColor",            "#121418");

const KEYS = {
  money:"CheatMod.SECTION[Money]",         infiniteMoney:"CheatMod.OPTION[InfiniteMoney]",
  milestones:"CheatMod.SECTION[Milestones]",unlockAll:"CheatMod.ACTION[UnlockAll]",
  advanceToTarget:"CheatMod.ACTION[AdvanceToTarget]",targetMilestone:"CheatMod.LABEL[TargetMilestone]",
  keepUnlocked:"CheatMod.OPTION[KeepUnlocked]",devTree:"CheatMod.SECTION[DevTree]",
  addPoints:"CheatMod.ACTION[AddPoints]",  demand:"CheatMod.SECTION[Demand]",
  residential:"CheatMod.DEMAND[Residential]",commercial:"CheatMod.DEMAND[Commercial]",
  industrial:"CheatMod.DEMAND[Industrial]",office:"CheatMod.DEMAND[Office]",
  low:"CheatMod.LEVEL[Low]",medium:"CheatMod.LEVEL[Medium]",high:"CheatMod.LEVEL[High]",
  forceBuild:"CheatMod.ACTION[ForceBuild]",
  construction:"CheatMod.SECTION[Construction]",
  instantBuild:"CheatMod.OPTION[InstantBuild]",buildSpeed:"CheatMod.LABEL[BuildSpeed]",
  population:"CheatMod.SECTION[Population]",
  moveIn:"CheatMod.OPTION[MoveIn]",moveInMult:"CheatMod.LABEL[MoveInMult]",
  tourists:"CheatMod.OPTION[Tourists]",touristMult:"CheatMod.LABEL[TouristMult]",
  maxHappiness:"CheatMod.OPTION[MaxHappiness]",
  richCitizens:"CheatMod.OPTION[RichCitizens]",
  maxEducation:"CheatMod.OPTION[MaxEducation]",
  infoMaxEducation:"CheatMod.INFO[MaxEducation]",
  buildings:"CheatMod.SECTION[Buildings]",
  upgradeAll:"CheatMod.ACTION[UpgradeAll]",
  fillStorage:"CheatMod.ACTION[FillStorage]",
  keepStorageFull:"CheatMod.OPTION[KeepStorageFull]",
  maxEfficiency:"CheatMod.OPTION[MaxEfficiency]",
  dragHint:"CheatMod.HINT[Drag]",
  resetOnNewMap:"CheatMod.OPTION[ResetOnNewMap]",
  infoInstantBuild:"CheatMod.INFO[InstantBuild]",
  infoHappiness:"CheatMod.INFO[Happiness]",
  infoRichCitizens:"CheatMod.INFO[RichCitizens]",
  infoEfficiency:"CheatMod.INFO[Efficiency]",
  infoResetOnNewMap:"CheatMod.INFO[ResetOnNewMap]",
  infoForceBuild:"CheatMod.INFO[ForceBuild]",
  infoUpgradeLoading:"CheatMod.INFO[UpgradeLoading]",
  tooltipDesc:"CheatMod.TOOLTIP[Desc]",
  appearance:"CheatMod.SECTION[Appearance]",
  panelColor:"CheatMod.LABEL[PanelColor]",
  colorReset:"CheatMod.ACTION[ColorReset]",
};

const EN: Record<string,string> = {
  [KEYS.money]:"Money",[KEYS.infiniteMoney]:"Infinite Money",
  [KEYS.milestones]:"Milestones",[KEYS.unlockAll]:"Unlock All",
  [KEYS.advanceToTarget]:"Advance to",[KEYS.targetMilestone]:"Target Milestone",
  [KEYS.keepUnlocked]:"Keep permanently unlocked",[KEYS.devTree]:"Development Tree",
  [KEYS.addPoints]:"+ 228 Points",[KEYS.demand]:"Demand",
  [KEYS.residential]:"Residential",[KEYS.commercial]:"Commercial",
  [KEYS.industrial]:"Industrial",[KEYS.office]:"Office",
  [KEYS.low]:"Low",[KEYS.medium]:"Medium",[KEYS.high]:"High",
  [KEYS.forceBuild]:"Force Build",
  [KEYS.construction]:"Construction Time",[KEYS.instantBuild]:"Instant Construction",[KEYS.buildSpeed]:"Build Speed",
  [KEYS.population]:"Population & Tourism",
  [KEYS.moveIn]:"Boost Move-In Rate",[KEYS.moveInMult]:"Move-In Multiplier",
  [KEYS.tourists]:"Boost Tourists",[KEYS.touristMult]:"Tourist Multiplier",
  [KEYS.maxHappiness]:"Max Citizen Happiness",
  [KEYS.richCitizens]:"Rich Citizens",
  [KEYS.maxEducation]:"Max Education",
  [KEYS.infoMaxEducation]:"All citizens have maximum education (Level 4)",
  [KEYS.buildings]:"Buildings",
  [KEYS.upgradeAll]:"Upgrade All to Level 5",
  [KEYS.fillStorage]:"Fill Storage",
  [KEYS.keepStorageFull]:"Keep storage permanently full",
  [KEYS.maxEfficiency]:"Max. Company Efficiency",
  [KEYS.dragHint]:"Drag title \u00b7 Corner to resize",
  [KEYS.resetOnNewMap]:"Reset cheats on new map",
  [KEYS.infoInstantBuild]:"Instant build active \u2014 buildings are completed immediately",
  [KEYS.infoHappiness]:"All citizens have maximum happiness (200)",
  [KEYS.infoRichCitizens]:"All households receive 500,000 money (every 5 game minutes)",
  [KEYS.infoEfficiency]:"Company efficiency is maximized",
  [KEYS.infoResetOnNewMap]:"All cheats will be disabled on next map load",
  [KEYS.infoForceBuild]:"Force Build active \u2014 Unlimited demand",
  [KEYS.infoUpgradeLoading]:"Loading\u2026",
  [KEYS.tooltipDesc]:"All-in-One Cheat Panel by Venatorax",
  [KEYS.appearance]:"Appearance",[KEYS.panelColor]:"Panel Color",[KEYS.colorReset]:"Reset",
};

const DE: Record<string,string> = {
  [KEYS.money]:"Geld",[KEYS.infiniteMoney]:"Unendlich Geld",
  [KEYS.milestones]:"Meilensteine",[KEYS.unlockAll]:"Alle freischalten",
  [KEYS.advanceToTarget]:"Voranschreiten bis",[KEYS.targetMilestone]:"Ziel-Meilenstein",
  [KEYS.keepUnlocked]:"Dauerhaft entsperrt halten",[KEYS.devTree]:"Entwicklungsbaum",
  [KEYS.addPoints]:"+ 228 Punkte",[KEYS.demand]:"Nachfrage",
  [KEYS.residential]:"Wohngeb\u00e4ude",[KEYS.commercial]:"Gewerbe",
  [KEYS.industrial]:"Industrie",[KEYS.office]:"B\u00fcro",
  [KEYS.low]:"Niedrig",[KEYS.medium]:"Mittel",[KEYS.high]:"Hoch",
  [KEYS.forceBuild]:"Sofort bauen",
  [KEYS.construction]:"Bauzeit",[KEYS.instantBuild]:"Sofortiger Bau",[KEYS.buildSpeed]:"Baugeschwindigkeit",
  [KEYS.population]:"Bev\u00f6lkerung & Tourismus",
  [KEYS.moveIn]:"Einzugsrate erh\u00f6hen",[KEYS.moveInMult]:"Einzugs-Multiplikator",
  [KEYS.tourists]:"Touristen erh\u00f6hen",[KEYS.touristMult]:"Touristen-Multiplikator",
  [KEYS.maxHappiness]:"Maximale B\u00fcrgerzufriedenheit",
  [KEYS.richCitizens]:"Reiche B\u00fcrger",
  [KEYS.maxEducation]:"Maximale Bildung",
  [KEYS.infoMaxEducation]:"Alle B\u00fcrger haben maximale Bildung (Stufe 4)",
  [KEYS.buildings]:"Geb\u00e4ude",
  [KEYS.upgradeAll]:"Alle auf Stufe 5 upgraden",
  [KEYS.fillStorage]:"Lager auff\u00fcllen",
  [KEYS.keepStorageFull]:"Lager dauerhaft voll halten",
  [KEYS.maxEfficiency]:"Max. Unternehmenseffizienz",
  [KEYS.dragHint]:"Titel ziehen \u00b7 Ecke zum Skalieren",
  [KEYS.resetOnNewMap]:"Cheats bei neuer Map zur\u00fccksetzen",
  [KEYS.infoInstantBuild]:"Sofortiger Bau aktiv \u2014 Geb\u00e4ude werden direkt fertiggestellt",
  [KEYS.infoHappiness]:"Alle B\u00fcrger haben maximale Zufriedenheit (200)",
  [KEYS.infoRichCitizens]:"Alle Haushalte erhalten 500.000 Geld (alle 5 Spielminuten)",
  [KEYS.infoEfficiency]:"Unternehmenseffizienz wird maximiert",
  [KEYS.infoResetOnNewMap]:"Beim n\u00e4chsten Mapstart werden alle Cheats ausgeschaltet",
  [KEYS.infoForceBuild]:"Sofort-Bau aktiv \u2014 Unbegrenzte Nachfrage",
  [KEYS.infoUpgradeLoading]:"Wird geladen\u2026",
  [KEYS.tooltipDesc]:"All-in-One Cheat-Panel von Venatorax",
  [KEYS.appearance]:"Erscheinungsbild",[KEYS.panelColor]:"Panel-Farbe",[KEYS.colorReset]:"Zur\u00fccksetzen",
};

const FR: Record<string,string> = {
  [KEYS.money]:"Argent",[KEYS.infiniteMoney]:"Argent infini",
  [KEYS.milestones]:"Jalons",[KEYS.unlockAll]:"Tout d\u00e9verrouiller",
  [KEYS.advanceToTarget]:"Avancer jusqu'\u00e0",[KEYS.targetMilestone]:"Jalon cible",
  [KEYS.keepUnlocked]:"Garder d\u00e9verrouill\u00e9",[KEYS.devTree]:"Arbre de d\u00e9veloppement",
  [KEYS.addPoints]:"+ 228 Points",[KEYS.demand]:"Demande",
  [KEYS.residential]:"R\u00e9sidentiel",[KEYS.commercial]:"Commercial",
  [KEYS.industrial]:"Industriel",[KEYS.office]:"Bureau",
  [KEYS.low]:"Bas",[KEYS.medium]:"Moyen",[KEYS.high]:"\u00c9lev\u00e9",
  [KEYS.forceBuild]:"Construction forc\u00e9e",
  [KEYS.construction]:"Temps de construction",[KEYS.instantBuild]:"Construction instantan\u00e9e",[KEYS.buildSpeed]:"Vitesse de construction",
  [KEYS.population]:"Population & Tourisme",
  [KEYS.moveIn]:"Acc\u00e9l\u00e9rer l'emm\u00e9nagement",[KEYS.moveInMult]:"Multiplicateur d'emm\u00e9nagement",
  [KEYS.tourists]:"Augmenter les touristes",[KEYS.touristMult]:"Multiplicateur de touristes",
  [KEYS.maxHappiness]:"Bonheur maximum",
  [KEYS.richCitizens]:"Citoyens riches",
  [KEYS.maxEducation]:"\u00c9ducation maximale",
  [KEYS.infoMaxEducation]:"Tous les citoyens ont une \u00e9ducation maximale (Niveau 4)",
  [KEYS.buildings]:"B\u00e2timents",
  [KEYS.upgradeAll]:"Tout upgrader niveau 5",
  [KEYS.fillStorage]:"Remplir le stockage",
  [KEYS.keepStorageFull]:"Maintenir le stockage plein",
  [KEYS.maxEfficiency]:"Efficacit\u00e9 max entreprises",
  [KEYS.dragHint]:"Glisser titre \u00b7 coin pour redimensionner",
  [KEYS.resetOnNewMap]:"R\u00e9initialiser les cheats sur nouvelle carte",
  [KEYS.infoInstantBuild]:"Construction instantan\u00e9e active \u2014 b\u00e2timents termin\u00e9s imm\u00e9diatement",
  [KEYS.infoHappiness]:"Tous les citoyens ont le bonheur maximum (200)",
  [KEYS.infoRichCitizens]:"Tous les m\u00e9nages re\u00e7oivent 500 000 d'argent (toutes les 5 minutes de jeu)",
  [KEYS.infoEfficiency]:"Efficacit\u00e9 des entreprises maximis\u00e9e",
  [KEYS.infoResetOnNewMap]:"Tous les cheats seront d\u00e9sactiv\u00e9s au prochain chargement de carte",
  [KEYS.infoForceBuild]:"Construction forc\u00e9e active \u2014 Demande illimit\u00e9e",
  [KEYS.infoUpgradeLoading]:"Chargement\u2026",
  [KEYS.tooltipDesc]:"Panneau de triche tout-en-un par Venatorax",
  [KEYS.appearance]:"Apparence",[KEYS.panelColor]:"Couleur du panneau",[KEYS.colorReset]:"R\u00e9initialiser",
};

const ES: Record<string,string> = {
  [KEYS.money]:"Dinero",[KEYS.infiniteMoney]:"Dinero infinito",
  [KEYS.milestones]:"Hitos",[KEYS.unlockAll]:"Desbloquear todo",
  [KEYS.advanceToTarget]:"Avanzar hasta",[KEYS.targetMilestone]:"Meta",
  [KEYS.keepUnlocked]:"Mantener desbloqueado",[KEYS.devTree]:"\u00c1rbol de desarrollo",
  [KEYS.addPoints]:"+ 228 Puntos",[KEYS.demand]:"Demanda",
  [KEYS.residential]:"Residencial",[KEYS.commercial]:"Comercial",
  [KEYS.industrial]:"Industrial",[KEYS.office]:"Oficina",
  [KEYS.low]:"Bajo",[KEYS.medium]:"Medio",[KEYS.high]:"Alto",
  [KEYS.forceBuild]:"Construcci\u00f3n forzada",
  [KEYS.construction]:"Tiempo de construcci\u00f3n",[KEYS.instantBuild]:"Construcci\u00f3n instant\u00e1nea",[KEYS.buildSpeed]:"Velocidad de construcci\u00f3n",
  [KEYS.population]:"Poblaci\u00f3n y Turismo",
  [KEYS.moveIn]:"Aumentar entrada de residentes",[KEYS.moveInMult]:"Multiplicador de entrada",
  [KEYS.tourists]:"Aumentar turistas",[KEYS.touristMult]:"Multiplicador de turistas",
  [KEYS.maxHappiness]:"Felicidad m\u00e1xima ciudadana",
  [KEYS.richCitizens]:"Ciudadanos ricos",
  [KEYS.maxEducation]:"Educaci\u00f3n m\u00e1xima",
  [KEYS.infoMaxEducation]:"Todos los ciudadanos tienen educaci\u00f3n m\u00e1xima (Nivel 4)",
  [KEYS.buildings]:"Edificios",
  [KEYS.upgradeAll]:"Mejorar todo a nivel 5",
  [KEYS.fillStorage]:"Llenar almac\u00e9n",
  [KEYS.keepStorageFull]:"Mantener almac\u00e9n siempre lleno",
  [KEYS.maxEfficiency]:"Eficiencia m\u00e1x empresas",
  [KEYS.dragHint]:"Arrastra t\u00edtulo \u00b7 esquina para redimensionar",
  [KEYS.resetOnNewMap]:"Reiniciar trucos en nuevo mapa",
  [KEYS.infoInstantBuild]:"Construcci\u00f3n instant\u00e1nea activa \u2014 los edificios se completan de inmediato",
  [KEYS.infoHappiness]:"Todos los ciudadanos tienen felicidad m\u00e1xima (200)",
  [KEYS.infoRichCitizens]:"Todos los hogares reciben 500.000 de dinero (cada 5 minutos de juego)",
  [KEYS.infoEfficiency]:"Eficiencia de empresas maximizada",
  [KEYS.infoResetOnNewMap]:"Todos los trucos se desactivar\u00e1n al cargar un nuevo mapa",
  [KEYS.infoForceBuild]:"Construcci\u00f3n forzada activa \u2014 Demanda ilimitada",
  [KEYS.infoUpgradeLoading]:"Cargando\u2026",
  [KEYS.tooltipDesc]:"Panel de trucos todo-en-uno por Venatorax",
  [KEYS.appearance]:"Apariencia",[KEYS.panelColor]:"Color del panel",[KEYS.colorReset]:"Restablecer",
};

const IT: Record<string,string> = {
  [KEYS.money]:"Denaro",[KEYS.infiniteMoney]:"Denaro infinito",
  [KEYS.milestones]:"Traguardi",[KEYS.unlockAll]:"Sblocca tutto",
  [KEYS.advanceToTarget]:"Avanza fino a",[KEYS.targetMilestone]:"Obiettivo",
  [KEYS.keepUnlocked]:"Mantieni sbloccato",[KEYS.devTree]:"Albero di sviluppo",
  [KEYS.addPoints]:"+ 228 Punti",[KEYS.demand]:"Domanda",
  [KEYS.residential]:"Residenziale",[KEYS.commercial]:"Commerciale",
  [KEYS.industrial]:"Industriale",[KEYS.office]:"Ufficio",
  [KEYS.low]:"Basso",[KEYS.medium]:"Medio",[KEYS.high]:"Alto",
  [KEYS.forceBuild]:"Costruzione forzata",
  [KEYS.construction]:"Tempo di costruzione",[KEYS.instantBuild]:"Costruzione istantanea",[KEYS.buildSpeed]:"Velocit\u00e0 di costruzione",
  [KEYS.population]:"Popolazione e turismo",
  [KEYS.moveIn]:"Aumenta tasso di trasferimento",[KEYS.moveInMult]:"Moltiplicatore trasferimento",
  [KEYS.tourists]:"Aumenta turisti",[KEYS.touristMult]:"Moltiplicatore turisti",
  [KEYS.maxHappiness]:"Felicit\u00e0 massima",
  [KEYS.richCitizens]:"Cittadini ricchi",
  [KEYS.maxEducation]:"Istruzione massima",
  [KEYS.infoMaxEducation]:"Tutti i cittadini hanno istruzione massima (Livello 4)",
  [KEYS.buildings]:"Edifici",
  [KEYS.upgradeAll]:"Tutto a livello 5",
  [KEYS.fillStorage]:"Riempi magazzino",
  [KEYS.keepStorageFull]:"Mantieni magazzino sempre pieno",
  [KEYS.maxEfficiency]:"Efficienza max aziende",
  [KEYS.dragHint]:"Trascina titolo \u00b7 angolo per ridimensionare",
  [KEYS.resetOnNewMap]:"Resetta trucchi su nuova mappa",
  [KEYS.infoInstantBuild]:"Costruzione istantanea attiva \u2014 edifici completati immediatamente",
  [KEYS.infoHappiness]:"Tutti i cittadini hanno felicit\u00e0 massima (200)",
  [KEYS.infoRichCitizens]:"Tutte le famiglie ricevono 500.000 (ogni 5 minuti di gioco)",
  [KEYS.infoEfficiency]:"Efficienza delle aziende massimizzata",
  [KEYS.infoResetOnNewMap]:"Tutti i trucchi saranno disattivati al prossimo caricamento",
  [KEYS.infoForceBuild]:"Costruzione forzata attiva \u2014 Domanda illimitata",
  [KEYS.infoUpgradeLoading]:"Caricamento\u2026",
  [KEYS.tooltipDesc]:"Pannello trucchi tutto-in-uno di Venatorax",
  [KEYS.appearance]:"Aspetto",[KEYS.panelColor]:"Colore del pannello",[KEYS.colorReset]:"Ripristina",
};

const JA: Record<string,string> = {
  [KEYS.money]:"\u8cc7\u91d1",[KEYS.infiniteMoney]:"\u7121\u9650\u306e\u8cc7\u91d1",
  [KEYS.milestones]:"\u30de\u30a4\u30eb\u30b9\u30c8\u30fc\u30f3",[KEYS.unlockAll]:"\u3059\u3079\u3066\u89e3\u9664",
  [KEYS.advanceToTarget]:"\u76ee\u6a19\u307e\u3067\u9032\u3081\u308b",[KEYS.targetMilestone]:"\u76ee\u6a19",
  [KEYS.keepUnlocked]:"\u6c38\u4e45\u306b\u89e3\u9664\u72b6\u614b\u3092\u7dad\u6301",[KEYS.devTree]:"\u958b\u767a\u30c4\u30ea\u30fc",
  [KEYS.addPoints]:"+ 228 \u30dd\u30a4\u30f3\u30c8",[KEYS.demand]:"\u9700\u8981",
  [KEYS.residential]:"\u4f4f\u5b85",[KEYS.commercial]:"\u5546\u696d",
  [KEYS.industrial]:"\u5de5\u696d",[KEYS.office]:"\u30aa\u30d5\u30a3\u30b9",
  [KEYS.low]:"\u4f4e",[KEYS.medium]:"\u4e2d",[KEYS.high]:"\u9ad8",
  [KEYS.forceBuild]:"\u5f37\u5236\u5efa\u8a2d",
  [KEYS.construction]:"\u5efa\u8a2d\u6642\u9593",[KEYS.instantBuild]:"\u5373\u6642\u5efa\u8a2d",[KEYS.buildSpeed]:"\u5efa\u8a2d\u901f\u5ea6",
  [KEYS.population]:"\u4eba\u53e3\u3068\u89b3\u5149",
  [KEYS.moveIn]:"\u8ee2\u5165\u7387\u3092\u4e0a\u3052\u308b",[KEYS.moveInMult]:"\u8ee2\u5165\u500d\u7387",
  [KEYS.tourists]:"\u89b3\u5149\u5ba2\u3092\u5897\u3084\u3059",[KEYS.touristMult]:"\u89b3\u5149\u5ba2\u500d\u7387",
  [KEYS.maxHappiness]:"\u5e02\u6c11\u306e\u5e78\u798f\u5ea6\u6700\u5927",
  [KEYS.richCitizens]:"\u88d5\u798f\u306a\u5e02\u6c11",
  [KEYS.maxEducation]:"\u6700\u9ad8\u6559\u80b2\u30ec\u30d9\u30eb",
  [KEYS.infoMaxEducation]:"\u5168\u5e02\u6c11\u304c\u6700\u9ad8\u6559\u80b2\u30ec\u30d9\u30eb\u3067\u3059\uff08\u30ec\u30d9\u30eb4\uff09",
  [KEYS.buildings]:"\u5efa\u7269",
  [KEYS.upgradeAll]:"\u3059\u3079\u3066\u30ec\u30d9\u30eb5\u306b\u30a2\u30c3\u30d7\u30b0\u30ec\u30fc\u30c9",
  [KEYS.fillStorage]:"\u5009\u5eab\u3092\u6e80\u305f\u3059",
  [KEYS.keepStorageFull]:"\u5009\u5eab\u3092\u5e38\u306b\u6e80\u676f\u306b\u7dad\u6301",
  [KEYS.maxEfficiency]:"\u4f01\u696d\u52b9\u7387\u6700\u5927",
  [KEYS.dragHint]:"\u30bf\u30a4\u30c8\u30eb\u3092\u30c9\u30e9\u30c3\u30b0 \u00b7 \u89d2\u3067\u30ea\u30b5\u30a4\u30ba",
  [KEYS.resetOnNewMap]:"\u65b0\u3057\u3044\u30de\u30c3\u30d7\u3067\u30c1\u30fc\u30c8\u3092\u30ea\u30bb\u30c3\u30c8",
  [KEYS.infoInstantBuild]:"\u5373\u6642\u5efa\u8a2d\u6709\u52b9 \u2014 \u5efa\u7269\u306f\u5373\u5ea7\u306b\u5b8c\u6210\u3057\u307e\u3059",
  [KEYS.infoHappiness]:"\u5168\u5e02\u6c11\u304c\u6700\u5927\u306e\u5e78\u798f\u5ea6\u3067\u3059 (200)",
  [KEYS.infoRichCitizens]:"\u5168\u4e16\u5e2f\u306b500,000\u306e\u8cc7\u91d1\u304c\u652f\u7d66\u3055\u308c\u307e\u3059 (\u30b2\u30fc\u30e0\u52055\u5206\u3054\u3068)",
  [KEYS.infoEfficiency]:"\u4f01\u696d\u52b9\u7387\u304c\u6700\u5927\u5316\u3055\u308c\u3066\u3044\u307e\u3059",
  [KEYS.infoResetOnNewMap]:"\u6b21\u306e\u30de\u30c3\u30d7\u30ed\u30fc\u30c9\u6642\u306b\u3059\u3079\u3066\u306e\u30c1\u30fc\u30c8\u304c\u7121\u52b9\u306b\u306a\u308a\u307e\u3059",
  [KEYS.infoForceBuild]:"\u5f37\u5236\u5efa\u8a2d\u6709\u52b9 \u2014 \u7121\u5236\u9650\u306e\u9700\u8981",
  [KEYS.infoUpgradeLoading]:"\u8aad\u307f\u8fbc\u307f\u4e2d\u2026",
  [KEYS.tooltipDesc]:"Venatorax\u306b\u3088\u308b\u30aa\u30fc\u30eb\u30a4\u30f3\u30ef\u30f3\u30fb\u30c1\u30fc\u30c8\u30d1\u30cd\u30eb",
  [KEYS.appearance]:"\u5916\u89b3",[KEYS.panelColor]:"\u30d1\u30cd\u30eb\u30ab\u30e9\u30fc",[KEYS.colorReset]:"\u30ea\u30bb\u30c3\u30c8",
};

const KO: Record<string,string> = {
  [KEYS.money]:"\uc790\uae08",[KEYS.infiniteMoney]:"\ubb34\ud55c \uc790\uae08",
  [KEYS.milestones]:"\ub9c8\uc77c\uc2a4\ud1a4",[KEYS.unlockAll]:"\uc804\uccb4 \uc7a0\uae08 \ud574\uc81c",
  [KEYS.advanceToTarget]:"\ubaa9\ud45c\uae4c\uc9c0 \uc9c4\ud589",[KEYS.targetMilestone]:"\ubaa9\ud45c",
  [KEYS.keepUnlocked]:"\uc601\uad6c \uc7a0\uae08 \ud574\uc81c \uc720\uc9c0",[KEYS.devTree]:"\uac1c\ubc1c \ud2b8\ub9ac",
  [KEYS.addPoints]:"+ 228 \ud3ec\uc778\ud2b8",[KEYS.demand]:"\uc218\uc694",
  [KEYS.residential]:"\uc8fc\uac70",[KEYS.commercial]:"\uc0c1\uc5c5",
  [KEYS.industrial]:"\uc0b0\uc5c5",[KEYS.office]:"\uc0ac\ubb34\uc2e4",
  [KEYS.low]:"\ub0ae\uc74c",[KEYS.medium]:"\uc911\uac04",[KEYS.high]:"\ub192\uc74c",
  [KEYS.forceBuild]:"\uac15\uc81c \uac74\uc124",
  [KEYS.construction]:"\uac74\uc124 \uc2dc\uac04",[KEYS.instantBuild]:"\uc989\uc2dc \uac74\uc124",[KEYS.buildSpeed]:"\uac74\uc124 \uc18d\ub3c4",
  [KEYS.population]:"\uc778\uad6c \ubc0f \uad00\uad11",
  [KEYS.moveIn]:"\uc804\uc785\ub960 \uc99d\uac00",[KEYS.moveInMult]:"\uc804\uc785 \ubc30\uc728",
  [KEYS.tourists]:"\uad00\uad11\uac1d \uc99d\uac00",[KEYS.touristMult]:"\uad00\uad11\uac1d \ubc30\uc728",
  [KEYS.maxHappiness]:"\uc2dc\ubbfc \ud589\ubcf5\ub3c4 \ucd5c\ub300",
  [KEYS.richCitizens]:"\ubd80\uc720\ud55c \uc2dc\ubbfc",
  [KEYS.maxEducation]:"\ucd5c\ub300 \uad50\uc721",
  [KEYS.infoMaxEducation]:"\ubaa8\ub4e0 \uc2dc\ubbfc\uc774 \ucd5c\ub300 \uad50\uc721 \uc218\uc900\uc785\ub2c8\ub2e4 (\ub808\ubca8 4)",
  [KEYS.buildings]:"\uac74\ubb3c",
  [KEYS.upgradeAll]:"\uc804\uccb4 \ub808\ubca8 5\ub85c \uc5c5\uadf8\ub808\uc774\ub4dc",
  [KEYS.fillStorage]:"\ucc3d\uace0 \ucc44\uc6b0\uae30",
  [KEYS.keepStorageFull]:"\ucc3d\uace0 \ud56d\uc0c1 \uac00\ub4dd \uc720\uc9c0",
  [KEYS.maxEfficiency]:"\uae30\uc5c5 \ud6a8\uc728 \ucd5c\ub300",
  [KEYS.dragHint]:"\uc81c\ubaa9\uc744 \ub4dc\ub798\uadf8 \u00b7 \ubaa8\uc11c\ub9ac\ub85c \ud06c\uae30 \uc870\uc808",
  [KEYS.resetOnNewMap]:"\uc0c8 \ub9f5\uc5d0\uc11c \uce58\ud2b8 \ucd08\uae30\ud654",
  [KEYS.infoInstantBuild]:"\uc989\uc2dc \uac74\uc124 \ud65c\uc131\ud654 \u2014 \uac74\ubb3c\uc774 \uc989\uc2dc \uc644\ub8cc\ub429\ub2c8\ub2e4",
  [KEYS.infoHappiness]:"\ubaa8\ub4e0 \uc2dc\ubbfc\uc774 \ucd5c\ub300 \ud589\ubcf5\ub3c4\uc785\ub2c8\ub2e4 (200)",
  [KEYS.infoRichCitizens]:"\ubaa8\ub4e0 \uac00\uad6c\uc5d0 500,000 \uc790\uae08\uc774 \uc9c0\uae09\ub429\ub2c8\ub2e4 (\uac8c\uc784 \ub0b4 5\ubd84\ub9c8\ub2e4)",
  [KEYS.infoEfficiency]:"\uae30\uc5c5 \ud6a8\uc728\uc774 \ucd5c\ub300\ud654\ub418\uc5c8\uc2b5\ub2c8\ub2e4",
  [KEYS.infoResetOnNewMap]:"\ub2e4\uc74c \ub9f5 \ub85c\ub4dc \uc2dc \ubaa8\ub4e0 \uce58\ud2b8\uac00 \ube44\ud65c\uc131\ud654\ub429\ub2c8\ub2e4",
  [KEYS.infoForceBuild]:"\uac15\uc81c \uac74\uc124 \ud65c\uc131\ud654 \u2014 \ubb34\uc81c\ud55c \uc218\uc694",
  [KEYS.infoUpgradeLoading]:"\ub85c\ub529 \uc911\u2026",
  [KEYS.tooltipDesc]:"Venatorax\uc758 \uc62c\uc778\uc6d0 \uce58\ud2b8 \ud328\ub110",
  [KEYS.appearance]:"\uc678\uad00",[KEYS.panelColor]:"\ud328\ub110 \uc0c9\uc0c1",[KEYS.colorReset]:"\ucd08\uae30\ud654",
};

const PL: Record<string,string> = {
  [KEYS.money]:"Pieni\u0105dze",[KEYS.infiniteMoney]:"Niesko\u0144czone pieni\u0105dze",
  [KEYS.milestones]:"Kamienie milowe",[KEYS.unlockAll]:"Odblokuj wszystko",
  [KEYS.advanceToTarget]:"Awansuj do",[KEYS.targetMilestone]:"Cel",
  [KEYS.keepUnlocked]:"Utrzymuj stale odblokowane",[KEYS.devTree]:"Drzewo rozwoju",
  [KEYS.addPoints]:"+ 228 Punkt\u00f3w",[KEYS.demand]:"Popyt",
  [KEYS.residential]:"Mieszkaniowy",[KEYS.commercial]:"Handlowy",
  [KEYS.industrial]:"Przemys\u0142owy",[KEYS.office]:"Biurowy",
  [KEYS.low]:"Niski",[KEYS.medium]:"\u015aredni",[KEYS.high]:"Wysoki",
  [KEYS.forceBuild]:"Wymu\u015b budow\u0119",
  [KEYS.construction]:"Czas budowy",[KEYS.instantBuild]:"Natychmiastowa budowa",[KEYS.buildSpeed]:"Pr\u0119dko\u015b\u0107 budowy",
  [KEYS.population]:"Populacja i turystyka",
  [KEYS.moveIn]:"Zwi\u0119ksz tempo zaludniania",[KEYS.moveInMult]:"Mno\u017cnik zaludniania",
  [KEYS.tourists]:"Zwi\u0119ksz turyst\u00f3w",[KEYS.touristMult]:"Mno\u017cnik turyst\u00f3w",
  [KEYS.maxHappiness]:"Maks. szcz\u0119\u015bcie obywateli",
  [KEYS.richCitizens]:"Bogaci obywatele",
  [KEYS.maxEducation]:"Maksymalna edukacja",
  [KEYS.infoMaxEducation]:"Wszyscy obywatele maj\u0105 maksymaln\u0105 edukacj\u0119 (Poziom 4)",
  [KEYS.buildings]:"Budynki",
  [KEYS.upgradeAll]:"Ulepsz wszystko do poziomu 5",
  [KEYS.fillStorage]:"Wype\u0142nij magazyn",
  [KEYS.keepStorageFull]:"Utrzymuj magazyn pe\u0142ny",
  [KEYS.maxEfficiency]:"Maks. wydajno\u015b\u0107 firm",
  [KEYS.dragHint]:"Przeci\u0105gnij tytu\u0142 \u00b7 r\u00f3g aby zmieni\u0107 rozmiar",
  [KEYS.resetOnNewMap]:"Resetuj cheaty na nowej mapie",
  [KEYS.infoInstantBuild]:"Natychmiastowa budowa aktywna \u2014 budynki s\u0105 uko\u0144czone od razu",
  [KEYS.infoHappiness]:"Wszyscy obywatele maj\u0105 maksymalne szcz\u0119\u015bcie (200)",
  [KEYS.infoRichCitizens]:"Wszystkie gospodarstwa otrzymuj\u0105 500 000 (co 5 minut gry)",
  [KEYS.infoEfficiency]:"Wydajno\u015b\u0107 firm zmaksymalizowana",
  [KEYS.infoResetOnNewMap]:"Wszystkie cheaty zostan\u0105 wy\u0142\u0105czone przy nast\u0119pnym \u0142adowaniu mapy",
  [KEYS.infoForceBuild]:"Wymuszenie budowy aktywne \u2014 Nieograniczony popyt",
  [KEYS.infoUpgradeLoading]:"\u0141adowanie\u2026",
  [KEYS.tooltipDesc]:"Panel wszystko-w-jednym autorstwa Venatorax",
  [KEYS.appearance]:"Wygl\u0105d",[KEYS.panelColor]:"Kolor panelu",[KEYS.colorReset]:"Resetuj",
};

const PT: Record<string,string> = {
  [KEYS.money]:"Dinheiro",[KEYS.infiniteMoney]:"Dinheiro infinito",
  [KEYS.milestones]:"Marcos",[KEYS.unlockAll]:"Desbloquear tudo",
  [KEYS.advanceToTarget]:"Avan\u00e7ar at\u00e9",[KEYS.targetMilestone]:"Meta",
  [KEYS.keepUnlocked]:"Manter desbloqueado",[KEYS.devTree]:"\u00c1rvore de desenvolvimento",
  [KEYS.addPoints]:"+ 228 Pontos",[KEYS.demand]:"Demanda",
  [KEYS.residential]:"Residencial",[KEYS.commercial]:"Comercial",
  [KEYS.industrial]:"Industrial",[KEYS.office]:"Escrit\u00f3rio",
  [KEYS.low]:"Baixo",[KEYS.medium]:"M\u00e9dio",[KEYS.high]:"Alto",
  [KEYS.forceBuild]:"Constru\u00e7\u00e3o for\u00e7ada",
  [KEYS.construction]:"Tempo de constru\u00e7\u00e3o",[KEYS.instantBuild]:"Constru\u00e7\u00e3o instant\u00e2nea",[KEYS.buildSpeed]:"Velocidade de constru\u00e7\u00e3o",
  [KEYS.population]:"Popula\u00e7\u00e3o e Turismo",
  [KEYS.moveIn]:"Aumentar taxa de mudan\u00e7a",[KEYS.moveInMult]:"Multiplicador de mudan\u00e7a",
  [KEYS.tourists]:"Aumentar turistas",[KEYS.touristMult]:"Multiplicador de turistas",
  [KEYS.maxHappiness]:"Felicidade m\u00e1xima cidad\u00e3os",
  [KEYS.richCitizens]:"Cidad\u00e3os ricos",
  [KEYS.maxEducation]:"Educa\u00e7\u00e3o m\u00e1xima",
  [KEYS.infoMaxEducation]:"Todos os cidad\u00e3os t\u00eam educa\u00e7\u00e3o m\u00e1xima (N\u00edvel 4)",
  [KEYS.buildings]:"Edif\u00edcios",
  [KEYS.upgradeAll]:"Melhorar tudo para n\u00edvel 5",
  [KEYS.fillStorage]:"Encher armaz\u00e9m",
  [KEYS.keepStorageFull]:"Manter armaz\u00e9m sempre cheio",
  [KEYS.maxEfficiency]:"Efici\u00eancia m\u00e1x empresas",
  [KEYS.dragHint]:"Arraste t\u00edtulo \u00b7 canto para redimensionar",
  [KEYS.resetOnNewMap]:"Resetar trapa\u00e7as em novo mapa",
  [KEYS.infoInstantBuild]:"Constru\u00e7\u00e3o instant\u00e2nea ativa \u2014 edif\u00edcios conclu\u00eddos imediatamente",
  [KEYS.infoHappiness]:"Todos os cidad\u00e3os t\u00eam felicidade m\u00e1xima (200)",
  [KEYS.infoRichCitizens]:"Todos os lares recebem 500.000 (a cada 5 minutos de jogo)",
  [KEYS.infoEfficiency]:"Efici\u00eancia das empresas maximizada",
  [KEYS.infoResetOnNewMap]:"Todas as trapa\u00e7as ser\u00e3o desativadas no pr\u00f3ximo carregamento de mapa",
  [KEYS.infoForceBuild]:"Constru\u00e7\u00e3o for\u00e7ada ativa \u2014 Demanda ilimitada",
  [KEYS.infoUpgradeLoading]:"Carregando\u2026",
  [KEYS.tooltipDesc]:"Painel de trapaças tudo-em-um por Venatorax",
  [KEYS.appearance]:"Apar\u00eancia",[KEYS.panelColor]:"Cor do painel",[KEYS.colorReset]:"Redefinir",
};

const RU: Record<string,string> = {
  [KEYS.money]:"\u0414\u0435\u043d\u044c\u0433\u0438",[KEYS.infiniteMoney]:"\u0411\u0435\u0441\u043a\u043e\u043d\u0435\u0447\u043d\u044b\u0435 \u0434\u0435\u043d\u044c\u0433\u0438",
  [KEYS.milestones]:"\u0412\u0435\u0445\u0438",[KEYS.unlockAll]:"\u0420\u0430\u0437\u0431\u043b\u043e\u043a\u0438\u0440\u043e\u0432\u0430\u0442\u044c \u0432\u0441\u0451",
  [KEYS.advanceToTarget]:"\u041f\u0440\u043e\u0434\u0432\u0438\u043d\u0443\u0442\u044c \u0434\u043e",[KEYS.targetMilestone]:"\u0426\u0435\u043b\u044c",
  [KEYS.keepUnlocked]:"\u0414\u0435\u0440\u0436\u0430\u0442\u044c \u0440\u0430\u0437\u0431\u043b\u043e\u043a\u0438\u0440\u043e\u0432\u0430\u043d\u043d\u044b\u043c",[KEYS.devTree]:"\u0414\u0435\u0440\u0435\u0432\u043e \u0440\u0430\u0437\u0432\u0438\u0442\u0438\u044f",
  [KEYS.addPoints]:"+ 228 \u041e\u0447\u043a\u043e\u0432",[KEYS.demand]:"\u0421\u043f\u0440\u043e\u0441",
  [KEYS.residential]:"\u0416\u0438\u043b\u043e\u0439",[KEYS.commercial]:"\u041a\u043e\u043c\u043c\u0435\u0440\u0447\u0435\u0441\u043a\u0438\u0439",
  [KEYS.industrial]:"\u041f\u0440\u043e\u043c\u044b\u0448\u043b\u0435\u043d\u043d\u044b\u0439",[KEYS.office]:"\u041e\u0444\u0438\u0441\u043d\u044b\u0439",
  [KEYS.low]:"\u041d\u0438\u0437\u043a\u0438\u0439",[KEYS.medium]:"\u0421\u0440\u0435\u0434\u043d\u0438\u0439",[KEYS.high]:"\u0412\u044b\u0441\u043e\u043a\u0438\u0439",
  [KEYS.forceBuild]:"\u041f\u0440\u0438\u043d\u0443\u0434\u0438\u0442\u0435\u043b\u044c\u043d\u0430\u044f \u0441\u0442\u0440\u043e\u0439\u043a\u0430",
  [KEYS.construction]:"\u0412\u0440\u0435\u043c\u044f \u0441\u0442\u0440\u043e\u0438\u0442\u0435\u043b\u044c\u0441\u0442\u0432\u0430",[KEYS.instantBuild]:"\u041c\u0433\u043d\u043e\u0432\u0435\u043d\u043d\u043e\u0435 \u0441\u0442\u0440\u043e\u0438\u0442\u0435\u043b\u044c\u0441\u0442\u0432\u043e",[KEYS.buildSpeed]:"\u0421\u043a\u043e\u0440\u043e\u0441\u0442\u044c \u0441\u0442\u0440\u043e\u0438\u0442\u0435\u043b\u044c\u0441\u0442\u0432\u0430",
  [KEYS.population]:"\u041d\u0430\u0441\u0435\u043b\u0435\u043d\u0438\u0435 \u0438 \u0442\u0443\u0440\u0438\u0437\u043c",
  [KEYS.moveIn]:"\u0423\u0432\u0435\u043b\u0438\u0447\u0438\u0442\u044c \u043f\u0440\u0438\u0442\u043e\u043a \u0436\u0438\u0442\u0435\u043b\u0435\u0439",[KEYS.moveInMult]:"\u041c\u043d\u043e\u0436\u0438\u0442\u0435\u043b\u044c \u043f\u0440\u0438\u0442\u043e\u043a\u0430",
  [KEYS.tourists]:"\u0423\u0432\u0435\u043b\u0438\u0447\u0438\u0442\u044c \u0442\u0443\u0440\u0438\u0441\u0442\u043e\u0432",[KEYS.touristMult]:"\u041c\u043d\u043e\u0436\u0438\u0442\u0435\u043b\u044c \u0442\u0443\u0440\u0438\u0441\u0442\u043e\u0432",
  [KEYS.maxHappiness]:"\u041c\u0430\u043a\u0441. \u0441\u0447\u0430\u0441\u0442\u044c\u0435 \u0433\u0440\u0430\u0436\u0434\u0430\u043d",
  [KEYS.richCitizens]:"\u0411\u043e\u0433\u0430\u0442\u044b\u0435 \u0433\u0440\u0430\u0436\u0434\u0430\u043d\u0435",
  [KEYS.maxEducation]:"\u041c\u0430\u043a\u0441\u0438\u043c\u0430\u043b\u044c\u043d\u043e\u0435 \u043e\u0431\u0440\u0430\u0437\u043e\u0432\u0430\u043d\u0438\u0435",
  [KEYS.infoMaxEducation]:"\u0412\u0441\u0435 \u0433\u0440\u0430\u0436\u0434\u0430\u043d\u0435 \u0438\u043c\u0435\u044e\u0442 \u043c\u0430\u043a\u0441\u0438\u043c\u0430\u043b\u044c\u043d\u043e\u0435 \u043e\u0431\u0440\u0430\u0437\u043e\u0432\u0430\u043d\u0438\u0435 (\u0423\u0440\u043e\u0432\u0435\u043d\u044c 4)",
  [KEYS.buildings]:"\u0417\u0434\u0430\u043d\u0438\u044f",
  [KEYS.upgradeAll]:"\u0423\u043b\u0443\u0447\u0448\u0438\u0442\u044c \u0432\u0441\u0451 \u0434\u043e \u0443\u0440\u043e\u0432\u043d\u044f 5",
  [KEYS.fillStorage]:"\u0417\u0430\u043f\u043e\u043b\u043d\u0438\u0442\u044c \u0441\u043a\u043b\u0430\u0434",
  [KEYS.keepStorageFull]:"\u0414\u0435\u0440\u0436\u0430\u0442\u044c \u0441\u043a\u043b\u0430\u0434 \u043f\u043e\u0441\u0442\u043e\u044f\u043d\u043d\u043e \u043f\u043e\u043b\u043d\u044b\u043c",
  [KEYS.maxEfficiency]:"\u041c\u0430\u043a\u0441. \u044d\u0444\u0444\u0435\u043a\u0442\u0438\u0432\u043d\u043e\u0441\u0442\u044c \u043a\u043e\u043c\u043f\u0430\u043d\u0438\u0439",
  [KEYS.dragHint]:"\u041f\u0435\u0440\u0435\u0442\u0430\u0449\u0438\u0442\u0435 \u0437\u0430\u0433\u043e\u043b\u043e\u0432\u043e\u043a \u00b7 \u0443\u0433\u043e\u043b \u0434\u043b\u044f \u0438\u0437\u043c\u0435\u043d\u0435\u043d\u0438\u044f \u0440\u0430\u0437\u043c\u0435\u0440\u0430",
  [KEYS.resetOnNewMap]:"\u0421\u0431\u0440\u043e\u0441\u0438\u0442\u044c \u0447\u0438\u0442\u044b \u043d\u0430 \u043d\u043e\u0432\u043e\u0439 \u043a\u0430\u0440\u0442\u0435",
  [KEYS.infoInstantBuild]:"\u041c\u0433\u043d\u043e\u0432\u0435\u043d\u043d\u043e\u0435 \u0441\u0442\u0440\u043e\u0438\u0442\u0435\u043b\u044c\u0441\u0442\u0432\u043e \u0430\u043a\u0442\u0438\u0432\u043d\u043e \u2014 \u0437\u0434\u0430\u043d\u0438\u044f \u0437\u0430\u0432\u0435\u0440\u0448\u0430\u044e\u0442\u0441\u044f \u043c\u0433\u043d\u043e\u0432\u0435\u043d\u043d\u043e",
  [KEYS.infoHappiness]:"\u0412\u0441\u0435 \u0433\u0440\u0430\u0436\u0434\u0430\u043d\u0435 \u0438\u043c\u0435\u044e\u0442 \u043c\u0430\u043a\u0441\u0438\u043c\u0430\u043b\u044c\u043d\u043e\u0435 \u0441\u0447\u0430\u0441\u0442\u044c\u0435 (200)",
  [KEYS.infoRichCitizens]:"\u0412\u0441\u0435 \u0434\u043e\u043c\u043e\u0445\u043e\u0437\u044f\u0439\u0441\u0442\u0432\u0430 \u043f\u043e\u043b\u0443\u0447\u0430\u044e\u0442 500 000 (\u043a\u0430\u0436\u0434\u044b\u0435 5 \u0438\u0433\u0440\u043e\u0432\u044b\u0445 \u043c\u0438\u043d\u0443\u0442)",
  [KEYS.infoEfficiency]:"\u042d\u0444\u0444\u0435\u043a\u0442\u0438\u0432\u043d\u043e\u0441\u0442\u044c \u043a\u043e\u043c\u043f\u0430\u043d\u0438\u0439 \u043c\u0430\u043a\u0441\u0438\u043c\u0430\u043b\u044c\u043d\u0430",
  [KEYS.infoResetOnNewMap]:"\u0412\u0441\u0435 \u0447\u0438\u0442\u044b \u0431\u0443\u0434\u0443\u0442 \u043e\u0442\u043a\u043b\u044e\u0447\u0435\u043d\u044b \u043f\u0440\u0438 \u0441\u043b\u0435\u0434\u0443\u044e\u0449\u0435\u0439 \u0437\u0430\u0433\u0440\u0443\u0437\u043a\u0435 \u043a\u0430\u0440\u0442\u044b",
  [KEYS.infoForceBuild]:"\u041f\u0440\u0438\u043d\u0443\u0434\u0438\u0442\u0435\u043b\u044c\u043d\u0430\u044f \u0441\u0442\u0440\u043e\u0439\u043a\u0430 \u0430\u043a\u0442\u0438\u0432\u043d\u0430 \u2014 \u041d\u0435\u043e\u0433\u0440\u0430\u043d\u0438\u0447\u0435\u043d\u043d\u044b\u0439 \u0441\u043f\u0440\u043e\u0441",
  [KEYS.infoUpgradeLoading]:"\u0417\u0430\u0433\u0440\u0443\u0437\u043a\u0430\u2026",
  [KEYS.tooltipDesc]:"\u0423\u043d\u0438\u0432\u0435\u0440\u0441\u0430\u043b\u044c\u043d\u0430\u044f \u043f\u0430\u043d\u0435\u043b\u044c \u0447\u0438\u0442\u043e\u0432 \u043e\u0442 Venatorax",
  [KEYS.appearance]:"\u0412\u043d\u0435\u0448\u043d\u0438\u0439 \u0432\u0438\u0434",[KEYS.panelColor]:"\u0426\u0432\u0435\u0442 \u043f\u0430\u043d\u0435\u043b\u0438",[KEYS.colorReset]:"\u0421\u0431\u0440\u043e\u0441",
};

const ZH_HANS: Record<string,string> = {
  [KEYS.money]:"\u8d44\u91d1",[KEYS.infiniteMoney]:"\u65e0\u9650\u8d44\u91d1",
  [KEYS.milestones]:"\u91cc\u7a0b\u7891",[KEYS.unlockAll]:"\u5168\u90e8\u89e3\u9501",
  [KEYS.advanceToTarget]:"\u63a8\u8fdb\u5230",[KEYS.targetMilestone]:"\u76ee\u6807",
  [KEYS.keepUnlocked]:"\u6c38\u4e45\u4fdd\u6301\u89e3\u9501",[KEYS.devTree]:"\u53d1\u5c55\u6811",
  [KEYS.addPoints]:"+ 228 \u70b9\u6570",[KEYS.demand]:"\u9700\u6c42",
  [KEYS.residential]:"\u4f4f\u5b85",[KEYS.commercial]:"\u5546\u4e1a",
  [KEYS.industrial]:"\u5de5\u4e1a",[KEYS.office]:"\u529e\u516c",
  [KEYS.low]:"\u4f4e",[KEYS.medium]:"\u4e2d",[KEYS.high]:"\u9ad8",
  [KEYS.forceBuild]:"\u5f3a\u5236\u5efa\u9020",
  [KEYS.construction]:"\u5efa\u9020\u65f6\u95f4",[KEYS.instantBuild]:"\u5373\u65f6\u5efa\u9020",[KEYS.buildSpeed]:"\u5efa\u9020\u901f\u5ea6",
  [KEYS.population]:"\u4eba\u53e3\u4e0e\u65c5\u6e38",
  [KEYS.moveIn]:"\u63d0\u9ad8\u8fc1\u5165\u7387",[KEYS.moveInMult]:"\u8fc1\u5165\u500d\u7387",
  [KEYS.tourists]:"\u589e\u52a0\u6e38\u5ba2",[KEYS.touristMult]:"\u6e38\u5ba2\u500d\u7387",
  [KEYS.maxHappiness]:"\u5e02\u6c11\u5e78\u798f\u5ea6\u6700\u5927",
  [KEYS.richCitizens]:"\u5bcc\u88d5\u5e02\u6c11",
  [KEYS.maxEducation]:"\u6700\u9ad8\u6559\u80b2\u6c34\u5e73",
  [KEYS.infoMaxEducation]:"\u6240\u6709\u5e02\u6c11\u62e5\u6709\u6700\u9ad8\u6559\u80b2\u6c34\u5e73\uff084\u7ea7\uff09",
  [KEYS.buildings]:"\u5efa\u7b51",
  [KEYS.upgradeAll]:"\u5168\u90e8\u5347\u7ea7\u52305\u7ea7",
  [KEYS.fillStorage]:"\u586b\u6ee1\u4ed3\u5e93",
  [KEYS.keepStorageFull]:"\u4fdd\u6301\u4ed3\u5e93\u59cb\u7ec8\u6ee1\u8f7d",
  [KEYS.maxEfficiency]:"\u4f01\u4e1a\u6548\u7387\u6700\u5927",
  [KEYS.dragHint]:"\u62d6\u52a8\u6807\u9898 \u00b7 \u89d2\u843d\u8c03\u6574\u5927\u5c0f",
  [KEYS.resetOnNewMap]:"\u65b0\u5730\u56fe\u65f6\u91cd\u7f6e\u4f5c\u5f0a",
  [KEYS.infoInstantBuild]:"\u5373\u65f6\u5efa\u9020\u5df2\u542f\u7528 \u2014 \u5efa\u7b51\u7acb\u5373\u5b8c\u6210",
  [KEYS.infoHappiness]:"\u6240\u6709\u5e02\u6c11\u62e5\u6709\u6700\u5927\u5e78\u798f\u5ea6 (200)",
  [KEYS.infoRichCitizens]:"\u6240\u6709\u5bb6\u5ead\u83b7\u5f97500,000\u8d44\u91d1 (\u6bcf\u6e38\u620f5\u5206\u949f)",
  [KEYS.infoEfficiency]:"\u4f01\u4e1a\u6548\u7387\u5df2\u6700\u5927\u5316",
  [KEYS.infoResetOnNewMap]:"\u4e0b\u6b21\u52a0\u8f7d\u5730\u56fe\u65f6\u6240\u6709\u4f5c\u5f0a\u5c06\u88ab\u7981\u7528",
  [KEYS.infoForceBuild]:"\u5f3a\u5236\u5efa\u9020\u5df2\u542f\u7528 \u2014 \u65e0\u9650\u5236\u9700\u6c42",
  [KEYS.infoUpgradeLoading]:"\u52a0\u8f7d\u4e2d\u2026",
  [KEYS.tooltipDesc]:"Venatorax\u7684\u5168\u80fd\u4f5c\u5f0a\u9762\u677f",
  [KEYS.appearance]:"\u5916\u89c2",[KEYS.panelColor]:"\u9762\u677f\u989c\u8272",[KEYS.colorReset]:"\u91cd\u7f6e",
};

const ZH_HANT: Record<string,string> = {
  [KEYS.money]:"\u8cc7\u91d1",[KEYS.infiniteMoney]:"\u7121\u9650\u8cc7\u91d1",
  [KEYS.milestones]:"\u91cc\u7a0b\u7891",[KEYS.unlockAll]:"\u5168\u90e8\u89e3\u9396",
  [KEYS.advanceToTarget]:"\u63a8\u9032\u5230",[KEYS.targetMilestone]:"\u76ee\u6a19",
  [KEYS.keepUnlocked]:"\u6c38\u4e45\u4fdd\u6301\u89e3\u9396",[KEYS.devTree]:"\u767c\u5c55\u6a39",
  [KEYS.addPoints]:"+ 228 \u9ede\u6578",[KEYS.demand]:"\u9700\u6c42",
  [KEYS.residential]:"\u4f4f\u5b85",[KEYS.commercial]:"\u5546\u696d",
  [KEYS.industrial]:"\u5de5\u696d",[KEYS.office]:"\u8fa6\u516c",
  [KEYS.low]:"\u4f4e",[KEYS.medium]:"\u4e2d",[KEYS.high]:"\u9ad8",
  [KEYS.forceBuild]:"\u5f37\u5236\u5efa\u9020",
  [KEYS.construction]:"\u5efa\u9020\u6642\u9593",[KEYS.instantBuild]:"\u5373\u6642\u5efa\u9020",[KEYS.buildSpeed]:"\u5efa\u9020\u901f\u5ea6",
  [KEYS.population]:"\u4eba\u53e3\u8207\u89c0\u5149",
  [KEYS.moveIn]:"\u63d0\u9ad8\u9077\u5165\u7387",[KEYS.moveInMult]:"\u9077\u5165\u500d\u7387",
  [KEYS.tourists]:"\u589e\u52a0\u904a\u5ba2",[KEYS.touristMult]:"\u904a\u5ba2\u500d\u7387",
  [KEYS.maxHappiness]:"\u5e02\u6c11\u5e78\u798f\u5ea6\u6700\u5927",
  [KEYS.richCitizens]:"\u5bcc\u88d5\u5e02\u6c11",
  [KEYS.maxEducation]:"\u6700\u9ad8\u6559\u80b2\u6c34\u6e96",
  [KEYS.infoMaxEducation]:"\u6240\u6709\u5e02\u6c11\u64c1\u6709\u6700\u9ad8\u6559\u80b2\u6c34\u6e96\uff084\u7d1a\uff09",
  [KEYS.buildings]:"\u5efa\u7bc9",
  [KEYS.upgradeAll]:"\u5168\u90e8\u5347\u7d1a\u52305\u7d1a",
  [KEYS.fillStorage]:"\u586b\u6eff\u5009\u5eab",
  [KEYS.keepStorageFull]:"\u4fdd\u6301\u5009\u5eab\u59cb\u7d42\u6eff\u8f09",
  [KEYS.maxEfficiency]:"\u4f01\u696d\u6548\u7387\u6700\u5927",
  [KEYS.dragHint]:"\u62d6\u52d5\u6a19\u984c \u00b7 \u89d2\u843d\u8abf\u6574\u5927\u5c0f",
  [KEYS.resetOnNewMap]:"\u65b0\u5730\u5716\u6642\u91cd\u7f6e\u4f5c\u5f0a",
  [KEYS.infoInstantBuild]:"\u5373\u6642\u5efa\u9020\u5df2\u555f\u7528 \u2014 \u5efa\u7bc9\u7acb\u5373\u5b8c\u6210",
  [KEYS.infoHappiness]:"\u6240\u6709\u5e02\u6c11\u64c1\u6709\u6700\u5927\u5e78\u798f\u5ea6 (200)",
  [KEYS.infoRichCitizens]:"\u6240\u6709\u5bb6\u5ead\u7372\u5f97500,000\u8cc7\u91d1 (\u6bcf\u904a\u62325\u5206\u9418)",
  [KEYS.infoEfficiency]:"\u4f01\u696d\u6548\u7387\u5df2\u6700\u5927\u5316",
  [KEYS.infoResetOnNewMap]:"\u4e0b\u6b21\u8f09\u5165\u5730\u5716\u6642\u6240\u6709\u4f5c\u5f0a\u5c07\u88ab\u7981\u7528",
  [KEYS.infoForceBuild]:"\u5f37\u5236\u5efa\u9020\u5df2\u555f\u7528 \u2014 \u7121\u9650\u5236\u9700\u6c42",
  [KEYS.infoUpgradeLoading]:"\u8f09\u5165\u4e2d\u2026",
  [KEYS.tooltipDesc]:"Venatorax\u7684\u5168\u80fd\u4f5c\u5f0a\u9762\u677f",
  [KEYS.appearance]:"\u5916\u89c0",[KEYS.panelColor]:"\u9762\u677f\u984f\u8272",[KEYS.colorReset]:"\u91cd\u7f6e",
};

const LOCALE_DICTS: Record<string, Record<string,string>> = {
  de: DE, fr: FR, es: ES, it: IT,
  ja: JA, ko: KO, pl: PL, pt: PT,
  ru: RU, zh: ZH_HANS, "zh-hant": ZH_HANT,
};

function useT() {
  const loc = useLocalization() as any;
  const translate = loc.translate;
  const locale: string = (loc.locale ?? loc.activeLocale ?? "").toLowerCase();
  const langCode = locale.split(/[-_]/)[0];
  const dictKey = langCode === "zh" && locale.indexOf("hant") !== -1 ? "zh-hant" : langCode;
  const dict = LOCALE_DICTS[dictKey];
  return (key: string): string => {
    if (dict && dict[key]) return dict[key];
    const fallback = EN[key] ?? key;
    const result = translate(key, fallback);
    if (dict && result === fallback && dict[key]) return dict[key];
    return (result ?? fallback) as string;
  };
}

const C = {
  bg:"rgba(18,20,24,0.98)", bgSection:"rgba(255,255,255,0.04)", bgHover:"rgba(255,255,255,0.08)",
  border:"rgba(255,255,255,0.09)", borderHover:"rgba(255,255,255,0.18)",
  textPrimary:"#dde1e7", textSecond:"#8a9098", textMuted:"#4a5060",
  accent:"#7aa2c8", green:"#6ec97a", greenDim:"rgba(110,201,122,0.11)",
  amber:"#d4b86a", amberDim:"rgba(212,184,106,0.11)",
  purple:"#aa88d4", purpleDim:"rgba(170,136,212,0.11)",
  red:"#d47070", redDim:"rgba(212,112,112,0.11)",
  orange:"#d49060", orangeDim:"rgba(212,144,96,0.11)",
  cyan:"#5bbfcf", cyanDim:"rgba(91,191,207,0.11)",
  teal:"#4dc4b0", tealDim:"rgba(77,196,176,0.11)",
  gold:"#c8a84b", goldDim:"rgba(200,168,75,0.11)",
};

const IconMoney  = () => (<svg width="13" height="13" viewBox="0 0 16 16" fill="none"><circle cx="8" cy="8" r="6.5" stroke={C.textSecond} strokeWidth="1.2"/><path d="M8 4v.8M8 11.2V12" stroke={C.textSecond} strokeWidth="1.2" strokeLinecap="round"/><path d="M6 7c0-.9.8-1.5 2-1.5s2 .5 2 1.3-.6 1.2-1.2 1.4L8 8.3c-.6.2-1.2.7-1.2 1.5s.8 1.3 2 1.3 2-.7 2-1.6" stroke={C.textSecond} strokeWidth="1.1" strokeLinecap="round"/></svg>);
const IconTrophy = () => (<svg width="13" height="13" viewBox="0 0 16 16" fill="none"><path d="M5 2h6v5a3 3 0 01-6 0V2z" stroke={C.textSecond} strokeWidth="1.2"/><path d="M2.5 3.5h2.5M11 3.5h2.5" stroke={C.textSecond} strokeWidth="1.2" strokeLinecap="round"/><path d="M2.5 3.5c0 2 .8 3 2.5 3.5M13.5 3.5c0 2-.8 3-2.5 3.5" stroke={C.textSecond} strokeWidth="1.1" strokeLinecap="round"/><path d="M8 10v3M6 14h4" stroke={C.textSecond} strokeWidth="1.2" strokeLinecap="round"/></svg>);
const IconTree   = () => (<svg width="13" height="13" viewBox="0 0 16 16" fill="none"><circle cx="8" cy="6" r="3" stroke={C.textSecond} strokeWidth="1.2"/><circle cx="8" cy="6" r="1" fill={C.textSecond}/><path d="M8 9v5M6 12l2 1.5L10 12" stroke={C.textSecond} strokeWidth="1.2" strokeLinecap="round"/></svg>);
const IconChart  = () => (<svg width="13" height="13" viewBox="0 0 16 16" fill="none"><path d="M2 12l3.5-4 3 2.5L13 4" stroke={C.textSecond} strokeWidth="1.2" strokeLinecap="round" strokeLinejoin="round"/><circle cx="5.5" cy="8" r="1.2" fill={C.textSecond}/><circle cx="8.5" cy="10.5" r="1.2" fill={C.textSecond}/><circle cx="13" cy="4" r="1.2" fill={C.textSecond}/></svg>);
const IconHammer = () => (<svg width="13" height="13" viewBox="0 0 16 16" fill="none"><rect x="2" y="3" width="7" height="4" rx="1" stroke={C.textSecond} strokeWidth="1.2"/><path d="M9 5l5 5" stroke={C.textSecond} strokeWidth="1.8" strokeLinecap="round"/><path d="M7 7l-4 5" stroke={C.textSecond} strokeWidth="1.2" strokeLinecap="round"/></svg>);
const IconPeople = () => (<svg width="13" height="13" viewBox="0 0 16 16" fill="none"><circle cx="6" cy="5" r="2" stroke={C.textSecond} strokeWidth="1.2"/><path d="M2 13c0-2.2 1.8-4 4-4s4 1.8 4 4" stroke={C.textSecond} strokeWidth="1.2" strokeLinecap="round"/><circle cx="11.5" cy="5" r="1.5" stroke={C.textSecond} strokeWidth="1.1"/><path d="M13 13c0-1.7-1-3-2.5-3" stroke={C.textSecond} strokeWidth="1.1" strokeLinecap="round"/></svg>);
const IconBuilding=() => (<svg width="13" height="13" viewBox="0 0 16 16" fill="none"><rect x="3" y="2" width="10" height="13" rx="1" stroke={C.textSecond} strokeWidth="1.2"/><path d="M6 5h1M9 5h1M6 8h1M9 8h1M6 11h1M9 11h1" stroke={C.textSecond} strokeWidth="1.2" strokeLinecap="round"/><path d="M6.5 15v-3h3v3" stroke={C.textSecond} strokeWidth="1.1"/></svg>);
const IconX       = () => (<svg width="9" height="9" viewBox="0 0 9 9" fill="none"><line x1="1.5" y1="1.5" x2="7.5" y2="7.5" stroke="currentColor" strokeWidth="1.6" strokeLinecap="round"/><line x1="7.5" y1="1.5" x2="1.5" y2="7.5" stroke="currentColor" strokeWidth="1.6" strokeLinecap="round"/></svg>);
const IconPalette = () => (<svg width="13" height="13" viewBox="0 0 16 16" fill="none"><path d="M8 2c-3.3 0-6 2.7-6 6 0 1.3.4 2.5 1.1 3.5.7 1 1.6 1.5 2.2.9.4-.4.5-1 .1-1.6-.3-.4-.3-.9 0-1.2.6-.6 1.6-.2 1.6.7 0 1 .9 1.7 1 1.7 3.3 0 6-2.7 6-6s-2.7-6-6-6z" stroke={C.textSecond} strokeWidth="1.2"/><circle cx="5.5" cy="7.5" r="1" fill={C.textSecond}/><circle cx="8" cy="5.5" r="1" fill={C.textSecond}/><circle cx="10.5" cy="7.5" r="1" fill={C.textSecond}/><circle cx="9" cy="10" r="1" fill={C.textSecond}/></svg>);

const DEFAULT_BG = "#121418";
const COLOR_PRESETS = ["#121418","#0d1521","#0d1a12","#1a0d1e","#1a0e0d","#0d1b1b","#1a1608","#1a0d17"];

function hexToRgba(hex: string, alpha = 0.98): string {
  const h = (hex || DEFAULT_BG).replace("#","");
  if (!/^[0-9a-fA-F]{6}$/.test(h)) return `rgba(18,20,24,${alpha})`;
  return `rgba(${parseInt(h.slice(0,2),16)},${parseInt(h.slice(2,4),16)},${parseInt(h.slice(4,6),16)},${alpha})`;
}

function AppearanceSection({color, onChange, t}: {color:string, onChange:(c:string)=>void, t:(k:string)=>string}) {
  const [inputVal, setInputVal] = React.useState(color);
  const colorInputRef = React.useRef<HTMLInputElement>(null);
  React.useEffect(()=>{ setInputVal(color); }, [color]);
  const previewColor = /^#[0-9a-fA-F]{6}$/.test(inputVal) ? inputVal : color;
  return (
    <Section icon={<IconPalette/>} title={t(KEYS.appearance)}>
      <div style={{display:"flex",gap:"6rem",alignItems:"center",marginBottom:"10rem",paddingLeft:"4rem"}}>
        {COLOR_PRESETS.map(hex => (
          <div key={hex} onClick={()=>{onChange(hex);setInputVal(hex);}} style={{
            width:"22rem",height:"22rem",borderRadius:"50%",background:hex,flexShrink:0,cursor:"pointer",
            border:hex===color?`2px solid ${C.textPrimary}`:`1px solid rgba(255,255,255,0.18)`,
            boxShadow:hex===color?"0 0 0 2px rgba(255,255,255,0.12)":"none",
          }}/>
        ))}
        <span onClick={()=>{onChange(DEFAULT_BG);setInputVal(DEFAULT_BG);}} style={{marginLeft:"auto",fontSize:"13rem",color:C.textMuted,cursor:"pointer",textDecoration:"underline",flexShrink:0,paddingRight:"32rem"}}>
          {t(KEYS.colorReset)}
        </span>
      </div>
      <div style={{display:"flex",gap:"10rem",alignItems:"center",paddingLeft:"4rem"}}>
        <div
          onClick={()=>colorInputRef.current?.click()}
          style={{position:"relative",width:"26rem",height:"26rem",borderRadius:"4rem",background:previewColor,border:`1px solid rgba(255,255,255,0.25)`,flexShrink:0,cursor:"pointer",overflow:"hidden"}}
          title={t(KEYS.panelColor)}
        >
          <input
            ref={colorInputRef}
            type="color"
            value={previewColor}
            onChange={e=>{const v=e.target.value;setInputVal(v);onChange(v);}}
            style={{position:"absolute",opacity:0,width:"100%",height:"100%",cursor:"pointer",border:"none",padding:0}}
          />
        </div>
        <input type="text" value={inputVal} maxLength={7}
          onChange={e=>{const v=e.target.value;setInputVal(v);if(/^#[0-9a-fA-F]{6}$/.test(v)){onChange(v);}}}
          onBlur={()=>{if(!/^#[0-9a-fA-F]{6}$/.test(inputVal))setInputVal(color);}}
          style={{background:"rgba(255,255,255,0.06)",border:`1px solid ${C.border}`,borderRadius:"4rem",color:C.textPrimary,fontSize:"11rem",padding:"4rem 8rem",width:"75rem",outline:"none"}}
        />
        <span style={{fontSize:"13rem",color:C.textMuted,marginLeft:"6rem"}}>{t(KEYS.panelColor)}</span>
      </div>
    </Section>
  );
}

type BtnVariant = "default"|"green"|"amber"|"purple"|"red"|"orange"|"cyan"|"teal"|"gold";
function Btn({onClick,children,variant="default",fullWidth=false,small=false,active=false}:{
  onClick:()=>void;children:React.ReactNode;variant?:BtnVariant;
  fullWidth?:boolean;small?:boolean;active?:boolean;
}) {
  const [hov,setHov]=useState(false);
  const [press,setPress]=useState(false);
  const cols:Record<BtnVariant,{bg:string;bgH:string;border:string;text:string;bgA:string;borderA:string}>={
    default:{bg:C.bgSection,bgH:C.bgHover,border:C.border,text:C.textPrimary,bgA:"rgba(255,255,255,0.12)",borderA:"rgba(255,255,255,0.30)"},
    green:  {bg:C.greenDim, bgH:"rgba(110,201,122,0.20)",border:"rgba(110,201,122,0.22)",text:C.green,  bgA:"rgba(110,201,122,0.28)",borderA:"rgba(110,201,122,0.55)"},
    amber:  {bg:C.amberDim, bgH:"rgba(212,184,106,0.20)",border:"rgba(212,184,106,0.25)",text:C.amber,  bgA:"rgba(212,184,106,0.28)",borderA:"rgba(212,184,106,0.55)"},
    purple: {bg:C.purpleDim,bgH:"rgba(170,136,212,0.20)",border:"rgba(170,136,212,0.25)",text:C.purple, bgA:"rgba(170,136,212,0.28)",borderA:"rgba(170,136,212,0.55)"},
    red:    {bg:C.redDim,   bgH:"rgba(212,112,112,0.20)",border:"rgba(212,112,112,0.25)",text:C.red,    bgA:"rgba(212,112,112,0.28)",borderA:"rgba(212,112,112,0.55)"},
    orange: {bg:C.orangeDim,bgH:"rgba(212,144,96,0.20)", border:"rgba(212,144,96,0.25)", text:C.orange, bgA:"rgba(212,144,96,0.28)", borderA:"rgba(212,144,96,0.55)"},
    cyan:   {bg:C.cyanDim,  bgH:"rgba(91,191,207,0.20)", border:"rgba(91,191,207,0.25)", text:C.cyan,   bgA:"rgba(91,191,207,0.30)", borderA:"rgba(91,191,207,0.60)"},
    teal:   {bg:C.tealDim,  bgH:"rgba(77,196,176,0.20)", border:"rgba(77,196,176,0.25)", text:C.teal,   bgA:"rgba(77,196,176,0.30)", borderA:"rgba(77,196,176,0.60)"},
    gold:   {bg:C.goldDim,  bgH:"rgba(200,168,75,0.20)", border:"rgba(200,168,75,0.25)", text:C.gold,   bgA:"rgba(200,168,75,0.30)", borderA:"rgba(200,168,75,0.60)"},
  };
  const col=cols[variant];
  const bg=active?col.bgA:(press||hov?col.bgH:col.bg);
  const border=active?col.borderA:(hov?C.borderHover:col.border);
  return (
    <div onClick={onClick}
      onMouseEnter={()=>setHov(true)} onMouseLeave={()=>{setHov(false);setPress(false);}}
      onMouseDown={()=>setPress(true)} onMouseUp={()=>setPress(false)}
      style={{flex:fullWidth?"0 0 auto":"1 1 auto",width:fullWidth?"100%":undefined,
        minHeight:small?"26rem":"30rem",padding:small?"4rem 10rem":"5rem 12rem",
        borderRadius:"5rem",background:bg,border:`1px solid ${border}`,
        color:col.text,fontSize:small?"11rem":"12rem",fontWeight:active?600:500,
        cursor:"pointer",display:"flex",alignItems:"center",justifyContent:"center",
        whiteSpace:"nowrap" as const,pointerEvents:"auto" as const,
        transform:press?"scale(0.97)":"scale(1)",transition:"all 0.10s ease",
      }}>{children}</div>
  );
}

function Toggle({value,onChange,accent=false}:{value:boolean;onChange:(v:boolean)=>void;accent?:boolean;}) {
  const onColor=accent?"rgba(212,144,96,0.75)":"rgba(122,162,200,0.70)";
  return (
    <div onClick={()=>onChange(!value)} style={{position:"relative",width:"34rem",height:"19rem",
      borderRadius:"10rem",background:value?onColor:"rgba(255,255,255,0.07)",
      border:`1px solid ${value?(accent?"rgba(212,144,96,0.5)":"rgba(122,162,200,0.45)"):C.border}`,
      cursor:"pointer",transition:"all 0.16s",flexShrink:0,pointerEvents:"auto" as const}}>
      <div style={{position:"absolute",top:"3rem",left:value?"17rem":"3rem",
        width:"11rem",height:"11rem",borderRadius:"50%",
        background:value?"#fff":C.textMuted,transition:"all 0.16s",
        boxShadow:value?"0 1px 3rem rgba(0,0,0,0.3)":"none"}}/>
    </div>
  );
}

function ToggleRow({label,value,onChange}:{label:string;value:boolean;onChange:(v:boolean)=>void;}) {
  return (
    <div style={{display:"flex",alignItems:"center",justifyContent:"space-between",gap:"12rem"}}>
      <span style={{fontSize:"13rem",color:value?C.textPrimary:C.textSecond,minWidth:0,
        overflow:"hidden",textOverflow:"ellipsis",whiteSpace:"nowrap" as const,
        transition:"color 0.14s"}}>{label}</span>
      <Toggle value={value} onChange={onChange}/>
    </div>
  );
}

function Slider({value,label,onChange,min=0,max=100}:{value:number;label?:string;onChange:(v:number)=>void;min?:number;max?:number;}) {
  const trackRef=useRef<HTMLDivElement>(null);
  const onChangeRef=useRef(onChange);
  onChangeRef.current=onChange;
  const onMouseDown=useCallback((e:React.MouseEvent)=>{
    e.stopPropagation();
    const calc=(clientX:number)=>{
      const el=trackRef.current;if(!el)return;
      const r=el.getBoundingClientRect();
      const pct=Math.max(0,Math.min(1,(clientX-r.left)/r.width));
      onChangeRef.current(Math.round(min+pct*(max-min)));
    };
    calc(e.clientX);
    const mv=(ev:MouseEvent)=>{calc(ev.clientX);};
    const up=()=>{window.removeEventListener("mousemove",mv);window.removeEventListener("mouseup",up);};
    window.addEventListener("mousemove",mv);window.addEventListener("mouseup",up);
  },[min,max]);
  const pct=((value-min)/(max-min))*100;
  const col=max===100?(value>=70?C.green:value>=35?C.accent:"#c07070"):C.accent;
  return (
    <div style={{display:"flex",flexDirection:"column" as const,gap:"5rem"}}>
      <div style={{display:"flex",justifyContent:"space-between",alignItems:"center"}}>
        {label&&<span style={{fontSize:"12rem",color:C.textMuted,minWidth:0,overflow:"hidden",textOverflow:"ellipsis",whiteSpace:"nowrap" as const}}>{label}</span>}
        <span style={{fontSize:"12rem",color:col,fontWeight:600,marginLeft:"auto",flexShrink:0}}>{value}{max===100?"%":"×"}</span>
      </div>
      <div style={{padding:"5rem 0",cursor:"pointer"}} onMouseDown={onMouseDown}>
        <div ref={trackRef} style={{position:"relative",width:"100%",height:"3rem",background:"rgba(255,255,255,0.07)",borderRadius:"2rem",overflow:"visible"}}>
          <div style={{position:"absolute",left:0,top:0,height:"100%",width:`${pct}%`,background:col,borderRadius:"2rem",opacity:0.65}}/>
          <div style={{position:"absolute",top:"50%",left:`${pct}%`,transform:"translate(-50%,-50%)",width:"11rem",height:"11rem",borderRadius:"50%",background:"#fff",border:`1.5px solid ${col}`,boxShadow:"0 1px 4rem rgba(0,0,0,0.4)",pointerEvents:"none" as const}}/>
        </div>
      </div>
    </div>
  );
}

function DemandBlock({t,label,override,onToggle,forceBuild,onForceBuild,forceBuildLabel,single,value,onValue,dual,valueLow,valueMed,valueHigh,onLow,onMed,onHigh,labelLow,labelMed,labelHigh}:{
  t:(k:string)=>string;label:string;override:boolean;onToggle:(v:boolean)=>void;forceBuild:boolean;onForceBuild:(v:boolean)=>void;forceBuildLabel:string;
  single?:boolean;value?:number;onValue?:(v:number)=>void;dual?:boolean;
  valueLow?:number;valueMed?:number;valueHigh?:number;onLow?:(v:number)=>void;onMed?:(v:number)=>void;onHigh?:(v:number)=>void;labelLow?:string;labelMed?:string;labelHigh?:string;
}) {
  const active=override||forceBuild;
  return (
    <div style={{borderRadius:"5rem",border:`1px solid ${active?"rgba(122,162,200,0.18)":C.border}`,background:active?"rgba(122,162,200,0.05)":C.bgSection,overflow:"hidden",transition:"all 0.16s"}}>
      <div style={{display:"flex",alignItems:"center",gap:"8rem",padding:"7rem 10rem"}}>
        <span style={{fontSize:"12rem",fontWeight:500,color:active?C.textPrimary:C.textSecond,flex:"1 1 0",minWidth:0,overflow:"hidden",textOverflow:"ellipsis",whiteSpace:"nowrap" as const,transition:"color 0.14s"}}>{label}</span>
        <Toggle value={forceBuild} onChange={onForceBuild} accent/>
        <span style={{fontSize:"13rem",color:forceBuild?C.orange:C.textMuted,flexShrink:0,minWidth:"60rem",marginLeft:"6rem",marginRight:"2rem"}}>{forceBuildLabel}</span>
        <Toggle value={override} onChange={onToggle}/>
      </div>
      {override&&!forceBuild&&(
        <div style={{padding:"0 10rem 10rem",display:"flex",flexDirection:"column" as const,gap:"8rem"}}>
          {single&&onValue&&value!==undefined&&<Slider value={value} onChange={onValue}/>}
          {dual&&<>{onLow&&<Slider value={valueLow??0} label={labelLow}  onChange={onLow}/>}{onMed&&<Slider value={valueMed??0} label={labelMed}  onChange={onMed}/>}{onHigh&&<Slider value={valueHigh??0} label={labelHigh} onChange={onHigh}/>}</>}
        </div>
      )}
      {forceBuild&&(<div style={{padding:"2rem 10rem 8rem"}}><div style={{fontSize:"13rem",color:C.orange,background:"rgba(212,144,96,0.08)",border:"1px solid rgba(212,144,96,0.15)",borderRadius:"4rem",padding:"4rem 8rem",textAlign:"center"}}>{t(KEYS.infoForceBuild)}</div></div>)}
    </div>
  );
}

function Section({icon,title,children}:{icon:React.ReactNode;title:string;children:React.ReactNode;}) {
  return (
    <div style={{borderRadius:"7rem",border:`1px solid ${C.border}`,background:C.bgSection,overflow:"hidden"}}>
      <div style={{display:"flex",alignItems:"center",padding:"7rem 12rem 6rem",borderBottom:`1px solid ${C.border}`,gap:"0"}}>
        <div style={{flexShrink:0,opacity:0.7,display:"flex",alignItems:"center",marginRight:"10rem"}}>{icon}</div>
        <span style={{fontSize:"11rem",fontWeight:600,color:C.textSecond,letterSpacing:"0.10em"}}>{title.toUpperCase()}</span>
      </div>
      <div style={{padding:"10rem 12rem",display:"flex",flexDirection:"column" as const,gap:"8rem"}}>{children}</div>
    </div>
  );
}

const Divider=()=><div style={{height:"1px",background:C.border,margin:"1rem 0"}}/>;

const SPEED_LABELS=["½×","1×","2×","4×"];
const SPEED_VARS: Array<"default"|"default"|"amber"|"orange"> = ["default","default","amber","orange"];

function ConstructionSection({t,instantConstruction,speedIndex,trig}:{t:(k:string)=>string;instantConstruction:boolean;speedIndex:number;trig:(name:string,...args:unknown[])=>void;}) {
  return (
    <Section icon={<IconHammer/>} title={t(KEYS.construction)}>
      <ToggleRow label={t(KEYS.instantBuild)} value={instantConstruction} onChange={v=>trig("SetInstantConstruction",v)}/>
      {instantConstruction&&(<div style={{fontSize:"13rem",color:C.cyan,background:"rgba(91,191,207,0.08)",border:"1px solid rgba(91,191,207,0.20)",borderRadius:"4rem",padding:"4rem 8rem",textAlign:"center"}}>{t(KEYS.infoInstantBuild)}</div>)}
      <Divider/>
      <div style={{display:"flex",flexDirection:"column" as const,gap:"6rem"}}>
        <span style={{fontSize:"11rem",color:C.textMuted}}>{t(KEYS.buildSpeed)}</span>
        <div style={{display:"flex",gap:"5rem"}}>
          {SPEED_LABELS.map((label,idx)=>(<Btn key={idx} variant={speedIndex===idx?"cyan":SPEED_VARS[idx]} active={speedIndex===idx} onClick={()=>trig("SetConstructionSpeedIndex",idx)}>{label}</Btn>))}
        </div>
      </div>
    </Section>
  );
}

function PopulationSection({t,overrideMoveIn,moveInMult,overrideTourists,touristMult,maxHappiness,richCitizens,maxEducation,trig}:{
  t:(k:string)=>string;overrideMoveIn:boolean;moveInMult:number;overrideTourists:boolean;touristMult:number;maxHappiness:boolean;richCitizens:boolean;maxEducation:boolean;trig:(name:string,...args:unknown[])=>void;
}) {
  return (
    <Section icon={<IconPeople/>} title={t(KEYS.population)}>
      <Divider/>
      <ToggleRow label={t(KEYS.moveIn)} value={overrideMoveIn} onChange={v=>trig("SetOverrideMoveIn",v)}/>
      {overrideMoveIn&&(<Slider value={moveInMult} label={t(KEYS.moveInMult)} min={1} max={10} onChange={v=>trig("SetMoveInMultiplier",v)}/>)}
      <Divider/>
      <ToggleRow label={t(KEYS.tourists)} value={overrideTourists} onChange={v=>trig("SetOverrideTourists",v)}/>
      {overrideTourists&&(<Slider value={touristMult} label={t(KEYS.touristMult)} min={1} max={10} onChange={v=>trig("SetTouristMultiplier",v)}/>)}
      <Divider/>
      <ToggleRow label={t(KEYS.maxHappiness)} value={maxHappiness} onChange={v=>trig("SetMaxHappiness",v)}/>
      {maxHappiness&&(<div style={{fontSize:"13rem",color:C.green,background:"rgba(110,201,122,0.08)",border:"1px solid rgba(110,201,122,0.20)",borderRadius:"4rem",padding:"4rem 8rem",textAlign:"center"}}>{t(KEYS.infoHappiness)}</div>)}
      <Divider/>
      <ToggleRow label={t(KEYS.richCitizens)} value={richCitizens} onChange={v=>trig("SetRichCitizens",v)}/>
      {richCitizens&&(<div style={{fontSize:"13rem",color:C.gold,background:"rgba(200,168,75,0.08)",border:"1px solid rgba(200,168,75,0.20)",borderRadius:"4rem",padding:"4rem 8rem",textAlign:"center"}}>{t(KEYS.infoRichCitizens)}</div>)}
      <Divider/>
      <ToggleRow label={t(KEYS.maxEducation)} value={maxEducation} onChange={v=>trig("SetMaxEducation",v)}/>
      {maxEducation&&(<div style={{fontSize:"13rem",color:C.gold,background:"rgba(200,168,75,0.08)",border:"1px solid rgba(200,168,75,0.20)",borderRadius:"4rem",padding:"4rem 8rem",textAlign:"center"}}>{t(KEYS.infoMaxEducation)}</div>)}
    </Section>
  );
}

function BuildingsSection({t,maxCompanyEfficiency,resetOnNewMap,buildingLevelAvailable,keepStorageFull,trig}:{t:(k:string)=>string;maxCompanyEfficiency:boolean;resetOnNewMap:boolean;buildingLevelAvailable:boolean;keepStorageFull:boolean;trig:(name:string,...args:unknown[])=>void;}) {
  return (
    <Section icon={<IconBuilding/>} title={t(KEYS.buildings)}>
      {buildingLevelAvailable
        ? <Btn variant="gold" fullWidth onClick={()=>trig("UpgradeAllBuildings")}>{t(KEYS.upgradeAll)}</Btn>
        : <div style={{borderRadius:"5rem",background:"rgba(200,168,75,0.05)",border:"1px solid rgba(200,168,75,0.15)",padding:"7rem 12rem",fontSize:"11rem",color:"rgba(200,168,75,0.45)",textAlign:"center" as const,cursor:"not-allowed",display:"flex",alignItems:"center",justifyContent:"center",gap:"6rem"}}>
            <svg width="11" height="11" viewBox="0 0 16 16" fill="none" style={{animation:"spin 1.2s linear infinite",flexShrink:0}}><circle cx="8" cy="8" r="5.5" stroke="rgba(200,168,75,0.35)" strokeWidth="1.5"/><path d="M8 2.5A5.5 5.5 0 0113.5 8" stroke="rgba(200,168,75,0.70)" strokeWidth="1.5" strokeLinecap="round"/></svg>
            {t(KEYS.upgradeAll)} – {t(KEYS.infoUpgradeLoading)}
            <style>{`@keyframes spin{from{transform:rotate(0deg)}to{transform:rotate(360deg)}}`}</style>
          </div>
      }
      <Divider/>
      <Btn variant="teal" fullWidth onClick={()=>trig("FillStorage")}>{t(KEYS.fillStorage)}</Btn>
      <ToggleRow label={t(KEYS.keepStorageFull)} value={keepStorageFull} onChange={v=>trig("SetKeepStorageFull",v)}/>
      <Divider/>
      <ToggleRow label={t(KEYS.maxEfficiency)} value={maxCompanyEfficiency} onChange={v=>trig("SetMaxCompanyEfficiency",v)}/>
      {maxCompanyEfficiency&&(<div style={{fontSize:"13rem",color:C.teal,background:"rgba(77,196,176,0.08)",border:"1px solid rgba(77,196,176,0.20)",borderRadius:"4rem",padding:"4rem 8rem",textAlign:"center"}}>{t(KEYS.infoEfficiency)}</div>)}
      <Divider/>
      <ToggleRow label={t(KEYS.resetOnNewMap)} value={resetOnNewMap} onChange={v=>trig("SetResetOnNewMap",v)}/>
      {resetOnNewMap&&(<div style={{fontSize:"13rem",color:C.textMuted,background:"rgba(255,255,255,0.04)",border:"1px solid rgba(255,255,255,0.08)",borderRadius:"4rem",padding:"4rem 8rem",textAlign:"center"}}>{t(KEYS.infoResetOnNewMap)}</div>)}
    </Section>
  );
}

function ResizeHandle({onResize}:{onResize:(dx:number,dy:number)=>void;}) {
  const [hov,setHov]=useState(false);
  const onMouseDown=useCallback((e:React.MouseEvent)=>{
    e.stopPropagation();
    let lx=e.clientX,ly=e.clientY;
    const mv=(ev:MouseEvent)=>{onResize(ev.clientX-lx,ev.clientY-ly);lx=ev.clientX;ly=ev.clientY;};
    const up=()=>{window.removeEventListener("mousemove",mv);window.removeEventListener("mouseup",up);};
    window.addEventListener("mousemove",mv);window.addEventListener("mouseup",up);
  },[onResize]);
  return (
    <div onMouseDown={onMouseDown} onMouseEnter={()=>setHov(true)} onMouseLeave={()=>setHov(false)}
      style={{position:"absolute",bottom:0,right:0,width:"18rem",height:"18rem",cursor:"nwse-resize",pointerEvents:"auto" as const,display:"flex",alignItems:"flex-end",justifyContent:"flex-end",padding:"4rem"}}>
      <svg width="7" height="7" viewBox="0 0 7 7"><line x1="7" y1="1" x2="1" y2="7" stroke={hov?C.textSecond:C.textMuted} strokeWidth="1.2" strokeLinecap="round"/><line x1="7" y1="4" x2="4" y2="7" stroke={hov?C.textSecond:C.textMuted} strokeWidth="1.2" strokeLinecap="round"/></svg>
    </div>
  );
}

function CloseBtn({onClick}:{onClick:()=>void;}) {
  const [hov,setHov]=useState(false);
  return (
    <div onClick={onClick} onMouseEnter={()=>setHov(true)} onMouseLeave={()=>setHov(false)}
      style={{width:"22rem",height:"22rem",borderRadius:"50%",background:hov?"rgba(190,70,70,0.35)":"rgba(255,255,255,0.05)",border:`1px solid ${hov?"rgba(210,80,80,0.45)":C.border}`,color:hov?"#ffaaaa":C.textSecond,display:"flex",alignItems:"center",justifyContent:"center",cursor:"pointer",pointerEvents:"auto" as const,transition:"all 0.10s"}}>
      <IconX/>
    </div>
  );
}

function CityForgeToolbarButton() {
  const t=useT();
  const panelVisible=useValue(PanelVisible$);
  const showButton  =useValue(ShowButton$);
  if(!showButton) return null;
  return (
    <Tooltip tooltip={
      <div style={{padding:"2rem 0"}}>
        <div style={{fontWeight:700,marginBottom:"4rem",fontSize:"13rem",letterSpacing:"0.04em"}}>CITYFORGE</div>
        <div style={{opacity:0.7,fontSize:"11rem"}}>{t(KEYS.tooltipDesc)}</div>
      </div>
    }>
      <FloatingButton src={cityForgeIcon} selected={panelVisible} onClick={()=>trigger("cheatmod","TogglePanel")}/>
    </Tooltip>
  );
}

const MIN_W=320,MAX_W=900,MIN_H=260,MARGIN=10;

function CityForgePanel() {
  const t=useT();
  const panelVisible          =useValue(PanelVisible$);
  const infiniteMoney         =useValue(InfiniteMoney$);
  const keepMilestones        =useValue(KeepMilestones$);
  const targetMilestone       =useValue(TargetMilestone$);
  const overrideRes           =useValue(OverrideRes$);
  const resLow                =useValue(ResLow$);
  const resMed                =useValue(ResMed$);
  const resHigh               =useValue(ResHigh$);
  const forceResBuild         =useValue(ForceResBuild$);
  const overrideCom           =useValue(OverrideCom$);
  const comValue              =useValue(ComValue$);
  const forceComBuild         =useValue(ForceComBuild$);
  const overrideInd           =useValue(OverrideInd$);
  const indValue              =useValue(IndValue$);
  const forceIndBuild         =useValue(ForceIndBuild$);
  const overrideOff           =useValue(OverrideOff$);
  const offValue              =useValue(OffValue$);
  const forceOffBuild         =useValue(ForceOffBuild$);
  const instantConstruction   =useValue(InstantConstruction$);
  const constructionSpeedIndex=useValue(ConstructionSpeedIndex$);
  const overrideMoveIn        =useValue(OverrideMoveIn$);
  const moveInMult            =useValue(MoveInMultiplier$);
  const overrideTourists      =useValue(OverrideTourists$);
  const touristMult           =useValue(TouristMultiplier$);
  const maxHappiness          =useValue(MaxHappiness$);
  const richCitizens          =useValue(RichCitizens$);
  const maxEducation          =useValue(MaxEducation$);
  const maxCompanyEfficiency  =useValue(MaxCompanyEfficiency$);
  const resetOnNewMap         =useValue(ResetOnNewMap$);
  const buildingLevelAvailable=useValue(BuildingLevelAvailable$);
  const keepStorageFull       =useValue(KeepStorageFull$);
  const panelBgColor          =useValue(PanelBgColor$);

  useEffect(() => {
    if (document.getElementById("cityforge-fonts")) return;
    const s = document.createElement("style");
    s.id = "cityforge-fonts";
    s.textContent = `.cityforge-root,.cityforge-root *{font-family:'Noto Sans JP','Noto Sans SC','Noto Sans TC','Noto Sans KR',sans-serif !important}`;
    document.head.appendChild(s);
  }, []);

  const dragOffset =useRef({x:0,y:0});
  const sizeRef    =useRef({w:380,h:600});
  const [pos,setPos]   =useState({x:320,y:100});
  const [size,setSize] =useState({w:380,h:600});
  sizeRef.current = size;

  const onHeaderMouseDown=useCallback((e:React.MouseEvent)=>{
    const ox=e.clientX-pos.x, oy=e.clientY-pos.y;
    const mv=(me:MouseEvent)=>{
      const s=sizeRef.current;
      setPos({x:Math.max(MARGIN,Math.min(window.innerWidth-s.w-MARGIN,me.clientX-ox)),y:Math.max(MARGIN,Math.min(window.innerHeight-s.h-MARGIN,me.clientY-oy))});
    };
    const up=()=>{window.removeEventListener("mousemove",mv);window.removeEventListener("mouseup",up);};
    window.addEventListener("mousemove",mv);
    window.addEventListener("mouseup",up);
  },[pos]);
  const onResize   =useCallback((dx:number,dy:number)=>{
    setSize(s=>({w:Math.max(MIN_W,Math.min(MAX_W,s.w+dx)),h:Math.max(MIN_H,Math.min(window.innerHeight-2*MARGIN,s.h+dy))}));
  },[]);

  const trig=(name:string,...args:unknown[])=>trigger("cheatmod",name,...args);
  const fbLabel=t(KEYS.forceBuild);
  const wide = size.w >= 480;
  const gap = 6;
  const pad = 8;
  const colPx = Math.floor((size.w - pad*2 - gap) / 2);

  const S=(style?:React.CSSProperties)=>({marginBottom:"6rem",...style});

  return (
    <div className="cityforge-root"
      style={{position:"fixed" as const,left:`${pos.x}px`,top:`${pos.y}px`,width:`${size.w}px`,height:`${size.h}px`,minWidth:`${MIN_W}px`,maxWidth:`${MAX_W}px`,minHeight:`${MIN_H}px`,background:hexToRgba(panelBgColor||DEFAULT_BG),border:`1px solid ${C.border}`,borderRadius:"10rem",boxShadow:"0 10rem 40rem rgba(0,0,0,0.75)",fontSize:"14rem",color:C.textPrimary,userSelect:"none" as const,zIndex:9999,overflow:"hidden",display:"flex",flexDirection:"column" as const,
        opacity: panelVisible ? 1 : 0,
        transform: panelVisible ? "scale(1) translateY(0rem)" : "scale(0.97) translateY(-8rem)",
        transformOrigin: "top center",
        transition: "opacity 0.16s ease, transform 0.16s ease",
        pointerEvents: panelVisible ? ("auto" as const) : ("none" as const)}}>


      <div onMouseDown={onHeaderMouseDown} style={{display:"flex",alignItems:"center",justifyContent:"space-between",padding:"10rem 14rem",borderBottom:`1px solid ${C.border}`,cursor:"grab",flexShrink:0}}>
        <div>
          <div style={{fontSize:"14rem",fontWeight:600,color:C.textPrimary,letterSpacing:"0.06em"}}>CITYFORGE</div>
          <div style={{fontSize:"11rem",color:C.textMuted,marginTop:"1rem"}}>by Venatorax</div>
        </div>
        <CloseBtn onClick={()=>trig("SetPanelVisible",false)}/>
      </div>

      <div style={{flex:"1 1 0",padding:`${pad}rem`,overflowY:"auto" as const,pointerEvents:"auto" as const}}>

        <div style={S()}>
          <Section icon={<IconMoney/>} title={t(KEYS.money)}>
            <div style={{display:"flex",marginBottom:"5rem"}}>
              {([["+ 100K",100_000],["+ 1M",1_000_000],["+ 10M",10_000_000],["+ 50M",50_000_000]] as [string,number][]).map(([l,a],i)=>(<div key={l} style={{flex:"1 1 auto",marginLeft:i>0?"5rem":"0"}}><Btn variant="green" onClick={()=>trig("AddMoney",a)}>{l}</Btn></div>))}
            </div>
            <div style={{display:"flex",marginBottom:"5rem"}}>
              {([["- 100K",-100_000],["- 1M",-1_000_000],["- 10M",-10_000_000],["- 50M",-50_000_000]] as [string,number][]).map(([l,a],i)=>(<div key={l} style={{flex:"1 1 auto",marginLeft:i>0?"5rem":"0"}}><Btn variant="red" onClick={()=>trig("AddMoney",a)}>{l}</Btn></div>))}
            </div>
            <Divider/>
            <ToggleRow label={t(KEYS.infiniteMoney)} value={infiniteMoney} onChange={v=>trig("SetInfiniteMoney",v)}/>
          </Section>
        </div>

        {wide ? (
          <div style={{display:"flex",flexDirection:"row" as const}}>
            <div style={{width:`${colPx}px`,flexShrink:0}}>
              <div style={S()}>
                <Section icon={<IconTrophy/>} title={t(KEYS.milestones)}>
                  <Slider value={targetMilestone} label={t(KEYS.targetMilestone)} min={1} max={20} onChange={v=>trig("SetTargetMilestone",v)}/>
                  <div style={{marginTop:"5rem"}}><Btn variant="amber" fullWidth onClick={()=>trig("AdvanceToTarget")}>{t(KEYS.advanceToTarget)} {targetMilestone}</Btn></div>
                  <Divider/>
                  <Btn variant="default" fullWidth onClick={()=>trig("UnlockMilestones")}>{t(KEYS.unlockAll)}</Btn>
                  <Divider/>
                  <ToggleRow label={t(KEYS.keepUnlocked)} value={keepMilestones} onChange={v=>trig("SetKeepMilestones",v)}/>
                </Section>
              </div>
              <div style={S()}>
                <BuildingsSection t={t} maxCompanyEfficiency={maxCompanyEfficiency} resetOnNewMap={resetOnNewMap} buildingLevelAvailable={buildingLevelAvailable} keepStorageFull={keepStorageFull} trig={trig}/>
              </div>
            </div>
            <div style={{width:`${colPx}px`,flexShrink:0,marginLeft:`${gap}rem`}}>
              <div style={S()}>
                <Section icon={<IconTree/>} title={t(KEYS.devTree)}>
                  <Btn variant="purple" fullWidth onClick={()=>trig("AddDevPoints")}>{t(KEYS.addPoints)}</Btn>
                </Section>
              </div>
              <div style={S()}>
                <ConstructionSection t={t} instantConstruction={instantConstruction} speedIndex={constructionSpeedIndex} trig={trig}/>
              </div>
              <div style={S()}>
                <PopulationSection t={t} overrideMoveIn={overrideMoveIn} moveInMult={moveInMult} overrideTourists={overrideTourists} touristMult={touristMult} maxHappiness={maxHappiness} richCitizens={richCitizens} maxEducation={maxEducation} trig={trig}/>
              </div>
            </div>
          </div>
        ) : (
          <div>
            <div style={S()}><Section icon={<IconTrophy/>} title={t(KEYS.milestones)}><Slider value={targetMilestone} label={t(KEYS.targetMilestone)} min={1} max={20} onChange={v=>trig("SetTargetMilestone",v)}/><div style={{marginTop:"5rem"}}><Btn variant="amber" fullWidth onClick={()=>trig("AdvanceToTarget")}>{t(KEYS.advanceToTarget)} {targetMilestone}</Btn></div><Divider/><Btn variant="default" fullWidth onClick={()=>trig("UnlockMilestones")}>{t(KEYS.unlockAll)}</Btn><Divider/><ToggleRow label={t(KEYS.keepUnlocked)} value={keepMilestones} onChange={v=>trig("SetKeepMilestones",v)}/></Section></div>
            <div style={S()}><Section icon={<IconTree/>} title={t(KEYS.devTree)}><Btn variant="purple" fullWidth onClick={()=>trig("AddDevPoints")}>{t(KEYS.addPoints)}</Btn></Section></div>
            <div style={S()}><BuildingsSection t={t} maxCompanyEfficiency={maxCompanyEfficiency} resetOnNewMap={resetOnNewMap} buildingLevelAvailable={buildingLevelAvailable} keepStorageFull={keepStorageFull} trig={trig}/></div>
            <div style={S()}><ConstructionSection t={t} instantConstruction={instantConstruction} speedIndex={constructionSpeedIndex} trig={trig}/></div>
            <div style={S()}><PopulationSection t={t} overrideMoveIn={overrideMoveIn} moveInMult={moveInMult} overrideTourists={overrideTourists} touristMult={touristMult} maxHappiness={maxHappiness} richCitizens={richCitizens} maxEducation={maxEducation} trig={trig}/></div>
          </div>
        )}

        <div style={S()}>
          <Section icon={<IconChart/>} title={t(KEYS.demand)}>
            <DemandBlock t={t} label={t(KEYS.residential)} override={overrideRes} onToggle={v=>trig("SetOverrideRes",v)} forceBuild={forceResBuild} onForceBuild={v=>trig("SetForceResBuild",v)} forceBuildLabel={fbLabel} dual valueLow={resLow} valueMed={resMed} valueHigh={resHigh} labelLow={t(KEYS.low)} labelMed={t(KEYS.medium)} labelHigh={t(KEYS.high)} onLow={v=>trig("SetResLow",v)} onMed={v=>trig("SetResMed",v)} onHigh={v=>trig("SetResHigh",v)}/>
            <DemandBlock t={t} label={t(KEYS.commercial)} override={overrideCom} onToggle={v=>trig("SetOverrideCom",v)} forceBuild={forceComBuild} onForceBuild={v=>trig("SetForceComBuild",v)} forceBuildLabel={fbLabel} single value={comValue} onValue={v=>trig("SetComValue",v)}/>
            <DemandBlock t={t} label={t(KEYS.industrial)} override={overrideInd} onToggle={v=>trig("SetOverrideInd",v)} forceBuild={forceIndBuild} onForceBuild={v=>trig("SetForceIndBuild",v)} forceBuildLabel={fbLabel} single value={indValue} onValue={v=>trig("SetIndValue",v)}/>
            <DemandBlock t={t} label={t(KEYS.office)}     override={overrideOff} onToggle={v=>trig("SetOverrideOff",v)} forceBuild={forceOffBuild} onForceBuild={v=>trig("SetForceOffBuild",v)} forceBuildLabel={fbLabel} single value={offValue} onValue={v=>trig("SetOffValue",v)}/>
          </Section>
        </div>

        <div style={S()}>
          <AppearanceSection color={panelBgColor||DEFAULT_BG} onChange={c=>trig("SetPanelBgColor",c)} t={t}/>
        </div>

        <div style={{textAlign:"center" as const,fontSize:"11rem",color:C.textMuted,padding:"14rem 0 6rem"}}>{t(KEYS.dragHint)}</div>
      </div>
      <ResizeHandle onResize={onResize}/>
    </div>
  );
}

const register: ModRegistrar = (moduleRegistry) => {
  moduleRegistry.append("GameTopLeft", CityForgeToolbarButton);
  moduleRegistry.append("Game", CityForgePanel);
};
export default register;
