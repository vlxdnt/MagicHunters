# Istoric Modificări (Changelog)

## [v0.0.9] - 20.03.2026
### Adaugat
- **Brightness Option:** adaugata optiunea de a modifica luminozitatea jocului

### De rezolvat:
- **Bug la animatii pe client:** animatia de sarit pentru Witch nu se aplica la Client
- **Bug la fade** fade ul se face doar pe host, pe client inca nu se aplica **REZOLVAT**
- **Bug la brightness** luminozitatea se schimba doar pe meniul de setari, nu overall **REZOLVAT**

### Next update:
- **Pause Menu** 

## [v0.0.8] - 19.03.2026
### Adaugat
- **Language Option:** Adaugata optiunea de a alege intre ro/en

### Modificat
- **Stil pentru text/titlu:** Modificat stilul si adaugat un material nou pentru titlu

## [v0.0.7] - 18.03.2026
### Adaugat
- **Fullscreen Option:** Adaugata optiunea de fullscreen in meniul de setari

### Modificat
- **Script-uri diferite:** Am creat 2 script-uri diferite pentru animatiile/abilitatile personajelor
- **Animatiile:** Rezolvat mai multe probleme ce tin de animatiile personajelor

## [v0.0.6] - 17.03.2026

### Adaugat
- **Disconnect handling:** Cand un jucator iese din joc, se intorc amandoi in meniu.

### Modificat
- **Optimizare:** Mutat crearea unui obiect MenuManager din Update in Start.

###
- **De reparat:** Cand un jucator iese din joc, nu functioneaza meniul (presimt ca e de la instanta meniului). Posibil fix: distrus instanta originala a meniului si creata o instanta noua cand se intoarce in menu.

## [v0.0.5] - 16.03.2026

### Adaugat
- **Settings Menu:** Adaugat Settings Menu cu diferite optiuni(volum on/off, slider pentru volum, rezolutie) ~ testat
- **Quit Button:** Adaugat optiunea de a iesi din joc

### Modificat
- **Button Sprite** Adaugat sprite-ul pentru butoane (apasat/neapasat)

### De facut:
- **Rezolutia** Setarea pentru rezolutie trebuie implementata, este doar de aspect acum.

### De adaugat pe viitor:
- **Fullscreen toggle**
- **Limba**
- **Luminozitate** ~ idee
- **Muzica de fundal/efecte sonore**
- **Fade intre scene**
- **Pause Panel**

## [v0.0.4] - 16.03.2026

### Adaugat
- **Harta:** Adaugat un level de testing

### Modificat
- **Jucatori:** Schimbat scale la jucatori la 1 1 1 si facut camera mai apropiata

### De reparat
- **Controale:** Nu mai merge jumpul ;(

## [V0.0.3.1] - 16.03.2026

### Reparat
- **Multiplayer:** Multiplayerul pe laptopuri diferite ar trebui sa functioneze acum.

## [v0.0.3] - 15.03.2026
### Adăugat
- **Font + Titlu:** Adaugat fontul tip 'Pixel Art', adaugat titlul cu animatie.
- **Loading Screen:** Se activeaza la apasarea butonului 'Host' si va tine atat timp cat se incarca serverul.

### Reparat
- **Delay-ul la animatii:** Rezolvat Animatorul pentru pisica.

## [v0.0.2] - 15.03.2026

### Adăugat
- **Buton Copy Code:** Adăugat un buton în lobby care copiază automat codul Relay în clipboard.
- **Validare Cod:** Adăugat un mesaj text de eroare în meniul principal dacă Clientul încearcă să se conecteze lăsând câmpul de cod gol, daca codul nu exista si daca lobby-ul e full.
- **Adaugat camere individuale:** Fiecare jucator are propria camera care il urmareste acum.

### Modificat
- **Fizică Jucători:** Jucătorii se pot mișca liber unul prin celălalt (dezactivat coliziunea dintre layerele "Player").

### Reparat
- **Bug Podea:** Jucătorii nu se mai blochează în colțurile tile-urilor de pe jos (adăugat Composite Collider 2D).
- **Bug NetworkAnimator:** Eliminat NullReferenceException-ul cauzat de sprite-urile temporare fără Animator **(am dat remove la Animator de la player 2 pana cand avem animatiile)**. 
- **Bug numar jucatori:** Contorizarea jucatorilor din lobby functioneaza corect acum.
- **Selectie jucati:** Reparat un bug unde nu puteai selecta pisica.

### De facut
- **Animatii:** Timerul la animatii la jucatori trebuie sincronizate (la vrajitoare dupa ce le adaugam).
- **Server:** In cazul in care unul din playeri se deconecteaza sa inchidem serverul si sa ii trimitem in meniul principal.
- **Loading screen:** De adaugat un loading panel

---

## [v0.0.1] - 14.03.2026

### Adăugat
- **Sistem de Lobby:** Jucătorii pot alege caractere diferite, cu distincție vizuală pe ecran (verde pt Host, albastru pt Client).
- **Butoane Lobby:** Adăugat buton de `Start` (doar pt Host, activ când lobby-ul e full) și `Quit Lobby` (resetează selecția la ieșire).
- **Contor Jucători:** Afișează numărul de jucători conectați curent în lobby.
- **Sistem de Spawn:** Creat `GameSpawner.cs` care încarcă `PlayerWitch` (Player 2) și `PlayerCat` (Player 1) în GameScene via `SpawnAsPlayerObject(clientId)`. Opțional, s-au setat puncte separate de spawn.
- **Input Independent:** Fiecare controlează doar propriul personaj pe calculatorul lui.
- **Sincronizare Mișcare:** Adaugat flip sprite sincronizat pe rețea folosind `NetworkVariable<bool>` pentru direcția de mișcare.

### Modificat
- **Conexiune Rețea:** Conexiunea nu se mai face direct prin IP, ci prin serviciul Unity Relay (generează un cod de 6 caractere pentru autentificare anonimă cu Unity Authentication Service).
- **Persistență UI:** Instanțiat `DontDestroyOnLoad` pe `UIManager` pentru a transmite informații fără a fi distruse la tranzitia dintre scene (LoadScene).
- **Verificare Podea:** Verificarea atingerii solului (grounded) se face acum prin `OverlapCircle`.

### De Făcut / Probleme Cunoscute
- De rezolvat contorizarea jucătorilor care nu se actualizează corect atunci când iese Clientul din lobby.
- De implementat butonul și Panel-ul pentru `Settings`.