# Cardapio RA MVP

Aplicativo Android em Unity para reconhecer etiquetas de pratos, apresentar uma
representacao 3D, exibir informacoes nutricionais e registrar uma avaliacao de
usabilidade SUS.

## Requisitos

- Unity `6000.4.5f1` ou outra versao Unity 6 compativel
- Modulo Android Build Support instalado
- Aparelho Android compativel com ARCore

## Primeira execucao

1. Abra esta pasta pelo Unity Hub.
2. Aguarde a restauracao dos pacotes.
3. Execute `Cardapio RA > Preparar projeto MVP` no menu do Editor.
4. Abra `Assets/Scenes/Mvp.unity`.
5. Pressione Play.

No Editor, use os botoes de simulacao para percorrer todo o fluxo sem camera.

## Ativar reconhecimento real

1. Importe uma imagem de etiqueta para cada prato.
2. Abra `Assets/App/Resources/MenuReferenceImages.asset`.
3. Adicione as imagens e use exatamente estes nomes:
   - `marker_prato_001`
   - `marker_prato_002`
   - `marker_prato_003`
4. Informe a largura fisica impressa de cada etiqueta em metros.
5. Gere e instale um build Android em aparelho compativel.

Os modelos atuais sao objetos geometricos temporarios. Para trocar um prato,
adicione um prefab em `Assets/App/Resources/Models` com o mesmo valor de
`modelResource` encontrado em `menu_catalog.json`.

## Dados coletados

Os eventos sao salvos como JSON Lines em
`Application.persistentDataPath/experiment-events.jsonl`. Nomes, e-mails e dados
de saude nao sao coletados.

