
#House Always Wins
A Spanish card game built in Unity where the house always has the edge — unless you're clever enough to beat it.
Mostrar imagen
Overview
House Always Wins is a single-player casino game based on the classic Spanish card game 7 y medio (7.5). The player must survive 7 days at the casino, facing increasingly difficult odds as the house manipulates the deck after each day.
The game features multiple mini-games, a shop with power-ups, and a dynamic probability system that changes with each round — making every run feel different.

Gameplay
🃏 7 y Medio (Main Game)

Draw cards from a Spanish deck and try to reach 7.5 without going over
Beat the house or lose your bet
Survive all 7 days to win — each day the house adjusts the deck probabilities, making it harder to predict what comes next

🎰 Mini-Games
GameDescriptionSlotsClassic slot machine with randomized outcomesCara o CruzCoin flip with variable multipliersRuletaRoulette with Spanish deck twist
🛍️ Shop & Wildcards
Earn currency and spend it on wildcards that change the flow of the game:

Extra Life — survive one more bust before game over
Tie the House — force a draw on any 7.5 hand
Card Removal — eliminate a specific card from the deck permanently
And more...


Technical Highlights

Dynamic probability system — the deck composition changes between days, requiring the player to adapt strategy
Data-driven content — game text and phrases loaded at runtime from external files using Unity's Resources system, making content easy to update without recompiling
Multiple game states — full state machine managing transitions between main game, mini-games, shop, pause, and game over screens
Modular wildcard system — each wildcard is implemented as an independent component, making it easy to add new ones
Unity UI system — fully built with Unity's UI Toolkit, including animated transitions and responsive layout


Built With

Unity (C#)
ShaderLab / HLSL — custom liquid shader for UI elements
Blender — 3D assets and environment props


Screenshots

(Add more screenshots here — main menu, shop, mini-games, game over screen)


Status
🚧 Currently in development — targeting a Steam release.

About
Made by Patricia MC
<table>
  <tr>
    <td><img src="/readme/calçot.png" width="2048" height="2048"></td>
    <td><img src="/readme/HCL7p8TWkAAyI-h.jpg" width="1350" height="799"></td>
    <td><img src="/readme/cocido.png" width="2048" height="2048"></td>
  </tr>
</table>

