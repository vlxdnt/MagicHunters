# Magic Hunters

Magic Hunters este un joc de tip action-adventure co-op asimetric, dezvoltat în Unity. Jucătorii trebuie să colaboreze folosind abilități magice și agilitate pentru a depăși obstacolele dintr-un castel plin de pericole.

## Povestea
Povestea urmărește o vrăjitoare și companionul ei fidel, o pisică neagră, în încercarea lor de a opri sora vrăjitoarei. Aceasta din urmă a fost coruptă de forțe întunecate și a devenit malefică, amenințând siguranța întregului regat. Cei doi protagoniști trebuie să traverseze niveluri complexe, să rezolve puzzle-uri mecanice și să supraviețuiască luptelor cu inamicii pentru a restabili echilibrul.

## Instrucțiuni de Lansare (Multiplayer)
Jocul utilizează sistemul Unity Netcode for GameObjects și funcționează în regim de rețea locală (LAN) prin utilizarea unui cod de acces (Join Code).

1. Lansați jocul pe ambele calculatoare conectate la aceeași rețea locală.
2. Jucătorul 1 (Host):
   - Selectați opțiunea "Start Host" din meniul principal.
   - Copiați codul generat (Join Code) care apare pe ecran și trimiteți-l partenerului de joc.
3. Jucătorul 2 (Client):
   - Introduceți codul primit în câmpul text corespunzător.
   - Selectați opțiunea "Join Client".
4. Odată ce ambii jucători sunt conectați, jucătorii trebuie să selecteze câte un personaj și hostul va trebui să apese start când ambii jucători sunt pregătiți.

## Controale

### General

| Tastă | Acțiune |
| :--- | :--- |
| ESC | Meniu de pauză |

### Pisica 
| Tastă | Acțiune |
| :--- | :--- |
| WASD | Mișcare |
| Space | Săritură (Apasă din nou în aer pentru Double Jump) |
| Shift | Dash (Deplasare rapidă/eschivă) |

### Vrăjitoarea 
| Tastă | Acțiune |
| :--- | :--- |
| WASD | Mișcare |
| Space | Săritură (Menține apăsat pentru Glide / Planare) |
| Q | Invizibilitate (Durează 5 secunde, inamicii pierd ținta) |
| H | Heal la ambii jucători (Cooldown 10 secunde, adaugă 50% din viața maximă la viața curentă) |
| E | Lansează o minge de foc |


## Cerințe de Sistem

### Minime
- Sistem de operare: Windows 10 (64-bit) sau mai nou
- Procesor: Dual Core 2.0 GHz sau echivalent
- Memorie: 4 GB RAM
- Grafică: Placă video integrată compatibilă cu DirectX 11 (1GB VRAM)
- Rezoluție: 1280 x 720 (720p)
- Rețea: Conexiune locală (LAN) activă

### Recomandate
- Procesor: Quad Core 2.5 GHz
- Memorie: 8 GB RAM
- Grafică: Placă video dedicată (seria GTX 1050 sau echivalent, 1GB VRAM)
- Rezoluție: 1920 x 1080 (1080p)

## Detalii Tehnice
- Motor grafic: Unity Engine
- Sincronizare rețea: Unity Netcode for GameObjects
- Pathfinding: A* Pathfinding Project
- Input System: New Unity Input System (Action-based)

## Credite

- **[vlxdnt](https://github.com/vlxdnt)**
- **[Cezar](https://github.com/CezarCodreanu)**
- **[Broko](https://github.com/BrokoDragon)**
