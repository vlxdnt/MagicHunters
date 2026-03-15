# Istoric Modificări (Changelog)

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