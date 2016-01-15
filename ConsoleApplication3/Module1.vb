Imports System.Net.Sockets
Imports System.Net
Imports System.Threading
Imports System.IO

Module Module1
    Dim flag As Boolean = True
    Dim port As String = "8080"
    Dim ListeClients As List(Of Client) 'Liste destinée à contenir les clients connectés
    'Dim coup() As String = {"e7e5", "f7f5", "g7g5", "h7h5"}
    'Dim i As Integer = 0
    Sub Main()

        'Crée le socket et l'IP EP
        Dim MonTcpListener As New TcpListener(IPAddress.Any, port)
        Dim monTcpClient As New TcpClient
        ListeClients = New List(Of Client) 'Initialise la liste

        MonTcpListener.Start()

        Console.WriteLine("Socket serveur initialisé sur le port " & port)

        While True 'Boucle à l'infini
            Console.WriteLine("En attente d'un client.")
            'Se met en attente de connexion et appelle TraitementConnexion() lors d'une connexion.
            monTcpClient = MonTcpListener.AcceptTcpClient() 'Bloquant tant que pas de connexion
            TraitementConnexion(monTcpClient) 'Traite la connexion du client
        End While

    End Sub

    Sub TraitementConnexion(ByVal SocketEnvoi As TcpClient)

        Console.WriteLine("Socket client connecté, création d'un thread.")

        Dim NouveauClient As New Client(SocketEnvoi) 'Crée une instance de « client »


        ListeClients.Add(NouveauClient) 'Ajoute le client à la liste

        'Crée un thread pour traiter ce client et le démarre
        Dim ThreadClient As New Thread(AddressOf NouveauClient.TraitementClient)
        ThreadClient.Start()
    End Sub

    Sub Broadcast(ByVal Message As String)

        'Écrit le message dans la console et l'envoie à tous les clients connectés
        Console.WriteLine("BROADCAST : " & Message)

        If flag Then
            ListeClients(0).EnvoiMessage(Message)
            flag = False
        Else
            ListeClients(1).EnvoiMessage(Message)
            flag = True
        End If

    End Sub

    Private Class Client
        Private _SocketClient As TcpClient 'Le socket du client
        'Constructeur
        Sub New(ByVal Sock As TcpClient)
            _SocketClient = Sock

        End Sub

        Sub TraitementClient()
            Console.WriteLine("Thread client lancé. ")
            Dim MonFlux As NetworkStream = _SocketClient.GetStream()
            Dim MonReader As StreamReader = New StreamReader(MonFlux)
            Dim MonWriter As StreamWriter = New StreamWriter(MonFlux)
            Dim Couleur() As String = {"Blanc", "Noir"}
            If flag Then
                MonWriter.WriteLine(Couleur(0))
                MonWriter.Flush()
                flag = False
            Else
                MonWriter.WriteLine(Couleur(1))
                MonWriter.Flush()
            End If
            While (_SocketClient.Connected)
                Dim Recu As String

                Try

                    Recu = MonReader.ReadLine()
                    'Message reçu
                Catch

                End Try
                Broadcast(Recu) 'Diffuse le message à tout le monde 
                'i = i + 1

            End While

        End Sub

        Sub EnvoiMessage(ByVal Message As String)
            Dim MonFlux As NetworkStream = _SocketClient.GetStream()
            Dim MonWriter As StreamWriter = New StreamWriter(MonFlux)

            MonWriter.WriteLine(Message)
            MonWriter.Flush()

        End Sub
    End Class

End Module