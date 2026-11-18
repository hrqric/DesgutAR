# Como colocar o DegustAR no celular Android (passo a passo)

Leia na ordem e **não pule etapas**. Cada etapa diz o que você deve ver na tela quando deu certo.

> **Dica que economiza tempo:** só **UMA pessoa** do grupo precisa fazer as partes 1 a 5 (instalar o Unity e gerar o app).
> Ela gera um arquivo chamado `DegustAR.apk` e manda para os outros. Quem recebe só faz a **Parte 6**
> (instalar o arquivo no celular), sem precisar do Unity.

---

## O que você precisa ter

| Item | Observação |
|---|---|
| Computador com Windows | Com uns **30 GB livres** no disco |
| Celular Android | Tem que ser compatível com **ARCore** (veja a Parte 0) |
| Cabo USB | Tem que ser cabo de **dados**. Cabo só de carregar não funciona |
| Internet | O Unity baixa vários arquivos grandes na primeira vez |
| Etiquetas | As imagens em `Assets/App/Resources/Markers/` (veja a Parte 7) |

---

## Parte 0 — O celular serve?

1. No celular, abra a **Play Store** e pesquise **"Serviços do Google Play para RA"** (em inglês: *Google Play Services for AR*).
2. Se aparecer o botão **Instalar** ou **Atualizar**, toque nele. Se o app **não existir** na loja para o seu celular, o aparelho **não é compatível**: use o de outra pessoa do grupo.
3. Confira também na lista oficial: pesquise no Google por **"ARCore supported devices"**.

---

## Parte 1 — Baixar o projeto

1. Instale o **Git**: https://git-scm.com/download/win (aperte "Next" até o fim).
2. Abra a pasta onde quer guardar o projeto. Clique com o botão direito num espaço vazio e escolha **Abrir no Terminal** (ou **Git Bash Here**).
3. Cole este comando e aperte Enter:

```bash
git clone https://github.com/hrqric/DesgutAR.git
```

4. Vai aparecer uma pasta chamada **DesgutAR**. Se o projeto já está no seu PC, pule esta parte.

> Se pedir login do GitHub, entre com sua conta. O dono do projeto precisa ter te adicionado como colaborador.

---

## Parte 2 — Instalar o Unity

1. Baixe o **Unity Hub**: https://unity.com/download e instale.
2. Abra o Unity Hub e entre com uma conta Unity (é grátis; crie uma se não tiver).
3. Descubra **qual versão** o projeto usa: abra o arquivo `ProjectSettings/ProjectVersion.txt` com o Bloco de Notas.
   Na primeira linha está algo como `m_EditorVersion: 6000.6.6f1`. **Anote esse número.**
4. No Unity Hub, clique em **Installs** (menu da esquerda) → **Install Editor**.
5. Se a versão que você anotou aparecer na lista, escolha ela. Se **não** aparecer, clique em **Archive** → **download archive** e procure pelo número.
6. Na tela de módulos, **marque estas caixas** (é o passo que mais gente erra):
   - ✅ **Android Build Support**
   - ✅ **OpenJDK**
   - ✅ **Android SDK & NDK Tools**
7. Clique em **Install** e espere terminar. Demora (pode ser 20 a 40 minutos).

**Deu certo quando:** a versão aparece em *Installs* com o ícone do Android embaixo.

---

## Parte 3 — Abrir o projeto no Unity

1. No Unity Hub, clique em **Projects** → **Add** → **Add project from disk**.
2. Escolha a pasta **DesgutAR** (a que tem dentro as pastas `Assets`, `Packages` e `ProjectSettings`).
   Atenção: é a pasta de fora, **não** entre em `Assets`.
3. Clique no projeto para abrir.
4. Se o Hub avisar que a versão do editor é diferente, escolha a versão que você instalou na Parte 2.
5. **Espere.** Na primeira vez o Unity importa tudo e baixa pacotes. Pode levar 10 a 20 minutos. Não feche.
6. Se aparecer a janela **"Enter Safe Mode?"**, clique em **Ignore** (ou **Continue**). Se aparecer de novo depois, copie o erro e mande para o dono do projeto.

**Deu certo quando:** o editor abriu e aparece o menu **Cardapio RA** na barra de cima.

---

## Parte 4 — Configurar o projeto para Android

Faça exatamente nesta ordem.

### 4.1 Preparar a cena
1. Na barra de cima, clique em **Cardapio RA** → **Preparar projeto MVP**.
2. Espere a mensagem verde no **Console** (aba embaixo) dizendo que a cena foi criada com sucesso.

### 4.2 Trocar a plataforma para Android
1. Clique em **File** → **Build Profiles**.
2. Na lista da esquerda, clique em **Android**.
3. Clique no botão **Switch Platform** e espere terminar (pode demorar alguns minutos).

### 4.3 Ligar o ARCore
1. Clique em **Edit** → **Project Settings**.
2. Na esquerda, clique em **XR Plug-in Management**.
3. Clique na aba com o **ícone do Android** (o robozinho).
4. Marque a caixa **ARCore**.
   **Não pule este passo:** sem ele o app abre, mas a câmera não liga e a tela fica azul escura, sem Realidade Aumentada.
   Depois de marcar, rode de novo **Cardapio RA → Preparar projeto MVP**.

### 4.4 Ajustar o Player
1. Ainda em *Project Settings*, clique em **Player** (na esquerda) e depois na aba do **Android**.
2. Abra a seta **Other Settings** e confira:
   - **Minimum API Level:** `Android 7.0 (API level 24)` ou maior.
   - **Scripting Backend:** `IL2CPP`.
   - **Target Architectures:** marque só **ARM64**.
   - **Active Input Handling:** `Input System Package (New)` (ou `Both`).
     Se mudar isso, o Unity pede para reiniciar. Clique em **Yes**.
3. Feche a janela de Project Settings.

### 4.5 Conferir a cena
1. Em **File** → **Build Profiles**, veja a lista **Scenes In Build**.
2. Tem que aparecer `Scenes/Mvp` **marcada**. Se não aparecer, clique em **Open Scene List** ou **Add Open Scenes**, depois de abrir a cena `Assets/Scenes/Mvp.unity` com duplo clique.

---

## Parte 5 — Gerar o app

Existem dois caminhos. Escolha **um**.

### Caminho A — Instalar direto no celular pelo cabo
1. No celular, ative o **modo desenvolvedor**:
   *Configurações → Sobre o telefone → toque 7 vezes em "Número da versão".*
   (Em alguns celulares fica em *Informações do software*.)
2. Volte para *Configurações → Sistema (ou Opções do desenvolvedor)* e ligue **Depuração USB**.
3. Ligue o celular no PC com o cabo. No celular vai aparecer **"Permitir depuração USB?"**. Marque *Sempre permitir* e toque em **Permitir**.
4. No Unity, abra **File → Build Profiles → Android**.
5. Em **Run Device**, escolha o seu celular. Se não aparecer, clique em **Refresh**.
6. Clique em **Build And Run**.
7. Escolha uma pasta para salvar o arquivo (pode criar uma chamada `Build`) e aperte **Save**.
8. Espere. A primeira vez demora **bastante** (10 a 30 minutos). O app abre sozinho no celular quando acaba.

### Caminho B — Gerar o arquivo APK e mandar para os outros
1. **File → Build Profiles → Android**.
2. Clique em **Build** (e não em Build And Run).
3. Nomeie o arquivo como `DegustAR.apk`, escolha uma pasta e salve.
4. Espere terminar. Mande o arquivo para o grupo por WhatsApp (como documento), Google Drive ou cabo USB.

**Deu certo quando:** o Unity termina sem erros vermelhos e existe um arquivo `.apk` na pasta.

---

## Parte 6 — Instalar o APK no celular (para quem recebeu o arquivo)

1. Passe o arquivo `DegustAR.apk` para o celular (WhatsApp, Drive ou cabo USB).
2. Abra o arquivo pelo app **Arquivos** (ou **Downloads**) do celular.
3. O Android vai avisar que é de fonte desconhecida. Toque em **Configurações** e ative **Permitir desta fonte**. Volte e toque em **Instalar**.
4. Se aparecer *"Bloqueado pelo Play Protect"*, toque em **Instalar mesmo assim**.
5. Abra o app **Cardapio RA**.
6. Quando pedir permissão da **câmera**, toque em **Permitir** (obrigatório).

---

## Parte 7 — Testar a Realidade Aumentada

1. Abra as imagens das etiquetas, que ficam na pasta `Assets/App/Resources/Markers/`:
   `marker_prato_001.png`, `marker_prato_002.png`, `marker_prato_003.png` e `marker_prato_004.png`.
2. Mostre uma na tela de **outro aparelho** (computador ou outro celular, com **brilho no máximo**) ou imprima com **12 cm de largura**.
3. Abra o app e aponte a câmera para a etiqueta, a uns **20 a 40 cm** de distância, com **boa luz** e a imagem **inteira** na câmera.
4. Devagar, sem tremer. O prato 3D deve aparecer em cima da etiqueta e o nome e as informações no painel.

| Etiqueta | Prato |
|---|---|
| `marker_prato_001` | Zinger Burger |
| `marker_prato_002` | Pizza de pepperoni |
| `marker_prato_003` | Cookie de chocolate |
| `marker_prato_004` | Ramen apimentado |

---

## Deu erro? Veja aqui

| Problema | O que fazer |
|---|---|
| O menu **Cardapio RA** não aparece | Abra a aba **Console**, veja se há erro vermelho e mande para o dono do projeto. Depois tente **Assets → Reimport All** |
| O celular não aparece em *Run Device* | Troque o cabo (use um de dados), aceite a **depuração USB** no celular, mude o USB para **Transferência de arquivos** e clique em **Refresh** |
| *Build* falha com erro de **SDK, NDK ou JDK** | Faltou instalar os módulos Android. Volte à Parte 2, passo 6 (*Installs → ⚙ → Add modules*) |
| Tela preta ou "dispositivo não suportado" | Instale ou atualize **Serviços do Google Play para RA** na Play Store, ou use outro celular |
| O app abre mas não pede a câmera | Vá em *Configurações → Apps → Cardapio RA → Permissões* e permita **Câmera** |
| O prato não aparece | Aumente a luz, afaste o celular, deixe a etiqueta inteira no quadro e teste outra etiqueta |
| O app está lento ou fecha sozinho | O modelo do ramen é pesado. Avise o dono do projeto para reduzir |
| *Instalação bloqueada* | Ative **Instalar apps desconhecidos** para o app que abriu o arquivo (Arquivos, Drive, etc.) |

---

## Quando algo der errado, mande isto

1. Print da aba **Console** do Unity (erros em vermelho).
2. Qual **parte e passo** você estava fazendo.
3. **Modelo do celular** e versão do Android.
