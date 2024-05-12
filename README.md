# Disaster Response Coordinator

O 'Disaster Response Coordinator' é um sistema interativo baseado em inteligência artificial para coordenação e suporte em situações de desastre. Utiliza a API do Gemini para facilitar a comunicação entre vítimas e serviços de emergência, integrando-se ao Sistema Integrado de Informações sobre Desastres v3. Um diferencial significativo deste sistema é que o diálogo acontece por meio do WhatsApp, uma plataforma amplamente utilizada, tornando o pedido por ajuda muito mais acessível. Essa integração garante que mais pessoas possam alcançar assistência de forma rápida e eficiente em momentos críticos.

## Imersão em Inteligência Artificial - 2ª Edição (Alura)

### Tecnologias Utilizadas
<div align="center">
  <img src="https://logospng.org/download/google-gemini/google-gemini-256.png" alt="logo gemini" width="150" style="margin-right: 20px;">
  <img src="https://i.imgur.com/ou7zb3O.png" alt="logo aspire" width="150" style="margin-right: 20px;">
  <img src="https://cdn.worldvectorlogo.com/logos/redis.svg" alt="logo redis" width="150" style="margin-right: 20px;">
  <img src="https://upload.wikimedia.org/wikipedia/commons/thumb/7/7d/Microsoft_.NET_logo.svg/64px-Microsoft_.NET_logo.svg.png" alt="logo .NET" width="100" style="margin-right: 20px;">
</div>

**Frameworks e APIs:**
- .NET 8, .NET Aspire, Blazor
- Redis, Google Places, Google Geo, ViaCep
- WhatsApp Cloud API, Gemini, Paraquemdoar.com.br

### Configuração e Execução
**Pré-requisitos:**
- Instale o .NET 8 e Docker. Verifique com `dotnet --version`.
- Configure o Aspire no seu ambiente .NET.

**Iniciar o Projeto:**
- Atualize `appsettings.json` no diretório API com as chaves necessárias.
- Execute `AppHost` para iniciar Redis, backend e frontend.

### Características e Funcionalidades
**Interações e Interfaces:**
- **Chat de Testes:** Interface de usuário otimizada para interação via browser para execução de testes sem configuração do Whatsapp.
- **Contexto Persistente:** Redis com `IChatCacheService` para manter o estado das conversas.

**Integrações Externas:**
- **Informações sobre Desastres:** Integração com S2iD v3 para dados em tempo real.
- **Localização:** Uso de Google Places e Google Geo para identificar hospitais e locais seguros.
- **Logística:** Consulta de endereços pelo CEP com ViaCep.

# Serviço de Chat

O `ChatService` é o coração da interação do usuário no "Disaster Response Coordinator". Este serviço gerencia a comunicação entre os usuários e o sistema, utilizando várias APIs e serviços para fornecer respostas precisas e oportunas durante emergências.

## Funcionalidades do Serviço
O `ChatService` implementa a interface `IChatService` e realiza múltiplas funções críticas:

### Inicialização e Gestão de Conversas
- **StartChat:** Inicia uma nova conversa ou retoma uma existente usando um identificador único (GUID). Configura o diálogo inicial com o usuário e prepara o sistema para responder a consultas baseadas em localização e emergência.

### Consultas Específicas
- **GetCurrentAddress:** Localiza um endereço via CEP utilizando o serviço `ViaCep`.
- **GetAvailableShelters:** Encontra hospitais e abrigos próximos baseando-se nas coordenadas obtidas pelo serviço `IGeocodingService`.
- **GetDonationPlaces:** Procura locais seguros para fazer doações com base em palavras-chave através do serviço `IBenfeitoriaService`.

### Processamento de Emergências
- **GetDesasters:** Analisa o risco de desastres em uma localidade específica combinando informações do usuário com dados de desastres obtidos pelo serviço `IS2iDService`.

## Armazenamento e Caching
- Utiliza `IDistributedCache` para armazenar informações que são frequentemente acessadas, como códigos de desastres (`Cobrades`), reduzindo a carga sobre as APIs externas e acelerando a resposta do sistema.

## Integrações Externas
- **GeminiClient:** Utilizado para construir e gerenciar conversas AI-driven.
- **Google Places e Google Geo:** Para a identificação precisa de locais específicos como hospitais e outros pontos de interesse.

## Exemplo de Uso
O método `SendMessage` demonstra a interação com o usuário:
- Recebe uma mensagem de um usuário e um GUID opcional.
- Inicia ou continua a conversa.
- Processa a mensagem através do `GeminiClient` usando opções de conclusão definidas.
- Salva a conversa atualizada no cache para garantir persistência e contexto.

### Estratégias de Caching
- **Cobrades:** Dados sobre categorias de desastres são cacheados e atualizados periodicamente para garantir a eficiência e a relevância das informações fornecidas aos usuários em situações de emergência.


## Contribua
1. Clone o repositório.
2. Crie uma branch para sua feature.
3. Faça alterações.
4. Envie um pull request.

Agradecemos seu apoio ao "Disaster Response Coordinator". Juntos, fazemos a diferença!

### Autor e Links
<div>
  <a href="https://www.linkedin.com/in/victor-verdoodt/"><img src="https://img.shields.io/badge/linkedin-0077B5.svg?style=for-the-badge&logo=linkedin&logoColor=white"></a>
  <a href="https://github.com/victorverdoodt/"><img src="https://img.shields.io/badge/github-3b4c52.svg?style=for-the-badge&logo=github&logoColor=white"></a>
  <a href="https://discord.com/channels/1228404913705451612/1228406162618060913/1238128762307219558"><img src="https://img.shields.io/badge/Discord-%235865F2.svg?style=for-the-badge&logo=discord&logoColor=white"></a>
</div>

### Galeria

![ImagemGitHub](https://github.com/victorverdoodt/Disaster-Response-Coordinator/assets/3966396/a1baf9b2-e8d3-4204-a027-9bc44399edee)

