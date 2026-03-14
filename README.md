# Magic Hunters
> schimbari 13.03 - 14.03

---

## 1. Crearea meniului
- conexiunea nu se mai face direct prin IP, ci prin servicul de Unity Relay
- serviciu ajuta Host-ul sa genereze un cod de 6 caractere care mai apoi ajuta la autentificare anonima cu UAS
- `DontDestroyOnLoad` pe UIManager e folosit pentru a transmite informatii intre scene (atunci cand se face tranzitia la loadScene)

---

## 2. Lobby / character selection
- jucatorii nu pot sa aleaga acelasi personaj, facand distinctia si colorarea vizuala(verde pt Host/albastru pt Client)
- adaugat contor pt jucatorii conectati in lobby
- adaugat butonul de `Start` vizibil doar pentru Host, se activeaza cand lobby-ul e full
- adaugat butonul de `Quit Lobby` vizibil pentru ambii jucatori (la iesirea din lobby a clientului, se reseteaza selectia caracterelor)

---

## 3. Player spawn
- creat `GameSpawner.cs` pe UIManager care incarca prefaburile caracterelor si le duce in GameScene
- doua prefaburi separate: `PlayerWitch` (Player2) si `PlayerCat` (Player1)
- spawn-ul se face prin `SpawnAsPlayerObject(clientId)`
- puncte de spawn separate pentru Host si Client in GameScene(sunt optionale)
---

## 4. Input
- fiecare controleaza doar propriul personaj
- rezolvat flip sprite sincronizat prin `NetworkVariable<bool>`(directia la miscare)
- verificare podea prin `OverlapCircle`

---

## 5. Bugs/de facut:
- de rezolvat contorizarea jucatorilor corecta atunci cand iese Clientul din lobby
- de implementat butonul si Panel-ul pt `Settings`
