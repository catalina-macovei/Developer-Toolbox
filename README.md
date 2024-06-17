# Developer-Toolbox :rocket:
***

## Membrii echipei

* Ciurescu Irina Alexandra

* Macovei Cătălina

* Stoinea Maria Miruna

* Toma Alexandra

## Instalare

* Clonare repository Developer Tools: https://github.com/catalina-macovei/Developer-Toolbox.git

* Clonare repository Backend Server pentru compilarea fișierelor python/ C++: https://github.com/catalina-macovei/backend-server.git

* Pentru server backend, se va porni serverul din terminal conform comenzilor indicate in Readme backend server.

* Pentru aplicația în .Net, se va rula cu build in Visual Studio. 


## Link-uri proiect

Demo: https://youtu.be/wtL93HdD8FU?si=qewP8Ewvj1DEg7ne

Trello Board: https://trello.com/invite/b/IkGNAbWn/ATTI8ac6152930936af6fce51826c086cd64D571E03D/proiect-mds-stack-overflow-developers-tool

Backend Server repository: https://github.com/catalina-macovei/backend-server

Repository pentru testele automate: https://github.com/stoineamiruna/Developer-ToolboxTests

## User stories:
* As an unregistred user, I want to see others' questions and answers.
* As user, I want to login.
* As user, I want to ask technical questions and receive answers.
* As user, I want to answer others' questions.
* As user, I want to have a profile.
* As user, I want to rate questions and answers.
* As user, I want to save questions and answers.
* As user, I want to be notified when I receive an answer.
* As user, I want to search a specific questions or a tag.
* As user, I want to browse a variety of coding exercises
* As user, I want to be able to select and attempt coding exercises directly within the app, with an integrated code editor.
* As user, I want to track my progress and performance over time. 
* As a moderator, I want to manage the content and exercises available on the platform
* As a moderator, I want to delete a post or a comment if it violates community guidelines. 
* As an administrator, I want to manage the content and exercises available on the platform
* As an administrator, I want to delete a post or a comment if it violates community guidelines. 
* As an administrator, I want to manage user roles and account details.
* As an administrator, I want the capability to delete user accounts.

## Backlog creation:

<p align="center">
  <img src="wwwroot\imgs\Wiki\Backlog.png" alt="Workflow Diagram">
</p>

## UML Diagram:
<p align="center">
  <img src="wwwroot\imgs\Wiki\UML.jpg" alt="UML">
</p>

## Gantt Chart:
<p align="center">
  <img src="wwwroot\imgs\Wiki\Gantt.svg" alt="Gantt Chart">
</p>

## Workflow Diagram:
<p align="center">
  <img src="wwwroot\imgs\Wiki\Workflow.svg" alt="Workflow Diagram">
</p>

## Entity Relationship Diagram:
<p align="center">
  <img src="wwwroot\imgs\Wiki\ERD.svg" alt="Workflow Diagram">
</p>

## Source control cu git

- Branch Creation

	Am folosit o integration branch pentru feature-uri numita development. Celelalte feature branches au urmatoarea conventie de nume: T de la “task”, urmat de ora si data crearii branch-ului. Regula de denumire este utila in urmarirea progresului proiectului.
	
	<img src="wwwroot\imgs\Wiki\Br.jpeg" alt="Branches">

- Commits

	<img src="wwwroot\imgs\Wiki\Commits.jpeg" alt="Commits">

- Git Conflicts and Merge
	
	Git Conflicts on Merge T10342905 into Development
	
	<img src="wwwroot\imgs\Wiki\Merge1.jpeg" alt="Merge Conflicts">

	 - Exercises Controller
	
		Vrem ambele versiuni: Test Cases si Solutions
		
		<img src="wwwroot\imgs\Wiki\Merge2.jpeg" alt="Merge Conflicts" style>

		Versiunea finala
		 
		<img src="wwwroot\imgs\Wiki\Merge3.jpeg" alt="Merge Conflicts">

	- Exercises Show
		
		Alegem incoming
		
		<img src="wwwroot\imgs\Wiki\Merge4.png" alt="Merge Conflicts">

	Astfel, am rezolvat cele doua conflicte si acum putem apasa Accept Merge.

- Pull requests

	<img src="wwwroot\imgs\Wiki\PR.png" alt="Pull requests">

	<img src="wwwroot\imgs\Wiki\PR2.png" alt="Pull requests">

## Teste Automate

- Pachetele xUnit și Moq

	Ca parte a procesului de dezvoltare, am realizat testarea automată, folosind cadrul de testare xUnit, care este un instrument popular pentru testarea unităților în aplicațiile .NET. In plus, am folosit biblioteca Moq pentru a simula dependențele, lucru a permis testarea componentelor în izolare, fără a se baza pe servicii sau baze de date externe.
	
	<p align="center">
		<img src="wwwroot\imgs\Wiki\Testare4.png" alt="Testare">
	</p>
- Realizarea și verificarea testelor create

	Aplicarea Design Pattern-ului Repository

	<p align="center">	
		<img src="wwwroot\imgs\Wiki\Testare2.png" alt="Testare">
	</p>
	
	Crearea efectivă a testelor

	<p align="center">	
		<img src="wwwroot\imgs\Wiki\Testare3.png" alt="Testare">
	</p>
	
	Verificarea și rularea testelor
	
	<p align="center">
	<img src="wwwroot\imgs\Wiki\Testare1.png" alt="Testare">
	</p>
	
## Raportare bug si rezolvare cu pull request

<img src="wwwroot\imgs\Wiki\Request.jpeg" alt="Pull requests">

Din backend server app:

<img src="wwwroot\imgs\Wiki\PullRequests2.png" alt="Pull requests">

<img src="wwwroot\imgs\Wiki\PullRequests3.png" alt="Pull requests">

<img src="wwwroot\imgs\Wiki\PullRequests4.png" alt="Pull requests">

<img src="wwwroot\imgs\Wiki\PullRequests5.png" alt="Pull requests">

## Comentarii cod

<img src="wwwroot\imgs\Wiki\Comentarii1.png" alt="Comentarii cod">

<img src="wwwroot\imgs\Wiki\Comentarii2.png" alt="Comentarii cod">

<img src="wwwroot\imgs\Wiki\Comentarii3.png" alt="Comentarii cod">

## Refactoring and code standards

- Cod modular, variabile denumite corespunzator, lungime redusa a functiilor, cod documentat, exception handling

	<img src="wwwroot\imgs\Wiki\Refactoring.png" alt="Refactoring">

	<img src="wwwroot\imgs\Wiki\Refactoring2.png" alt="Refactoring">

	<img src="wwwroot\imgs\Wiki\Refactoring3.png" alt="Refactoring">

- Exemplu de refactoring
	
	Pentru testarea surselor trimise de utilizatori ca solutii la exercitiile de pe platforma, trebuie preluate in backend server test cases pentru exercitiul la care se face submit. 
	Apoi acestea trebuie parsate, astfel incat sa fie extrase, pentru fiecare test case, inputul si outputul. Pe acestea le colectam intr-o structura de date care sa faciliteze accesul.
	Initial, am construit o functie prepare_test_cases care spargea continutul test case-ului (string) pentru a extrage informatiile necesare si pentru a face validarile. 
	In acest caz, existau variabile care aveau unicul rol de a retine string-uri intermediare din parsare, iar validarea esua daca nu se respecta cu strictete formatul cerut (de ex, nu se acceptau mai multe linii goale intre teste).
	In urma unui refactoring, am optat pentru o functie mai clara si pentru o validare mai flexibila, folosing expresii regulate.
	
	<img src="wwwroot\imgs\Wiki\Refactoring4.png" alt="Refactoring">

## Design Patterns

1. **Model-View-Controller (MVC) Pattern**

   - **Model**: Reprezintă datele aplicației și logica de business. Modelele noastre sunt:
     - `Answer`
     - `ApplicationUser`
     - `Bookmark`
     - `Category`
     - `Exercise`
     - `Question`
     - `QuestionTag`
     - `Reaction`
     - `Solution`
     - `Tag`
   - **View**: Reprezintă interfața cu utilizatorul. Exemple de view-uri sunt:
     - `Index`
     - `Edit`
     - `New`
     - `Show`
   - **Controller**: Gestionază interacțiunile utilizatorului, manipulează modelul și returnează view-uri corespunzătoare.

2. **Repository Pattern**

   - **ApplicationDbContext**: Acest context de bază de date acționează ca un repository pentru modelele tale, gestionând accesul la date și operațiile CRUD. Repository Pattern ajută la izolarea logicii de acces la date de restul aplicației, facilitând testarea și menținerea.

3. **Dependency Injection (DI)**

   - **Constructor Injection**: Controllerele primesc `ApplicationDbContext` prin constructor. DI ajută la reducerea dependențelor hard-coded și crește testabilitatea și flexibilitatea aplicației.

4. **Unit of Work Pattern**

   - **ApplicationDbContext**: Acționează adesea ca o unitate de lucru, asigurându-se că modificările aduse entităților sunt gestionate ca o singură tranzacție. Metoda `SaveChanges` aplică toate modificările la baza de date într-o singură tranzacție.

5. **ViewBag/TempData (State Management Patterns)**

   - **ViewBag și TempData**: Sunt utilizate pentru a transfera date între controller și view sau între diferite request-uri.

6. **Builder Pattern**

   - **SeedData**: Deși nu este un exemplu clasic al Builder Pattern, codul din `SeedData` pentru crearea și configurarea utilizatorilor poate fi văzut ca o formă simplificată a acestui pattern, unde obiectele complexe sunt construite pas cu pas.

  
  ## AI Tools
     Pe parcursul dezvoltării proiectului, am utilizat instrumente AI precum ChatGPT și Microsoft Copilot. Am folosit în principal ChatGPT pentru a cere explicații și îndrumări în situații care ne depășeau cunoștințele, precum și pentru a ne ajuta să generăm teste mai rapid.


