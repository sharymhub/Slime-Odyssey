Public Class Form3
    Private Const JumpHeight As Integer = 100 ' Altura máxima del salto
    Private Const JumpSpeed As Integer = 5 ' Velocidad del salto
    Private Const MoveSpeed As Integer = 5 ' Velocidad de movimiento horizontal
    Private Const GravitySpeed As Integer = 4 ' Velocidad de la gravedad
    Private jumpDistance As Integer ' Distancia recorrida durante el salto
    Private jumping As Boolean ' Indica si está saltando
    Private initialPosition As Point ' Posición inicial del PLayer1
    Private Const EnemyMoveSpeed As Integer = 3
    Private victoriaFormMostrado As Boolean = False

    Private enemyDirections As New Dictionary(Of PictureBox, Boolean)
    Private pocionesRecolectadas As Integer = 0
    Private pocionesTotales As Integer = 4

    Private WithEvents TimerEnemyMove As New Timer()
    Private WithEvents TimerPociones As New Timer()

    ' Variables para vidas y puntuación
    Private vidas As Integer = 3
    Private puntuacion As Integer = 0

    Public Sub New()
        InitializeComponent()

        initialPosition = Player1.Location
        PictureBox1.Tag = "Plataforma"
        PictureBox2.Tag = "Plataforma"
        PictureBox3.Tag = "Plataforma"
        PictureBox4.Tag = "Plataforma"
        PictureBox5.Tag = "Plataforma"
        PictureBox6.Tag = "Plataforma"
        PictureBox7.Tag = "Plataforma"
        PictureBox8.Tag = "Plataforma"
        PictureBox9.Tag = "Plataforma"
        PictureBox10.Tag = "Plataforma"
        PictureBox11.Tag = "Plataforma"
        PictureBox14.Tag = "Plataforma"
        PictureBox15.Tag = "Plataforma"
        PictureBox16.Tag = "Plataforma"
        PictureBox17.Tag = "Plataforma"
        PictureBox18.Tag = "Plataforma"
        PictureBox19.Tag = "Plataforma"
        PictureBox20.Tag = "Plataforma"
        PictureBox21.Tag = "Plataforma"
        PictureBox22.Tag = "Plataforma"
        PictureBox32.Tag = "Plataforma"
        PictureBox33.Tag = "Plataforma"
        PictureBox34.Tag = "Plataforma"
        PictureBox35.Tag = "Plataforma"
        PictureBox44.Tag = "Plataforma"

        PictureBox12.Tag = "Caja"
        PictureBox13.Tag = "Caja"
        PictureBox23.Tag = "Caja"
        PictureBox24.Tag = "Caja"
        PictureBox25.Tag = "Caja"
        PictureBox26.Tag = "Caja"
        PictureBox27.Tag = "Caja"

        PictureBox28.Tag = "Pocion"
        PictureBox29.Tag = "Pocion"
        PictureBox30.Tag = "Pocion"
        PictureBox31.Tag = "Pocion"

        PictureBox46.Tag = "Enemigo"
        PictureBox47.Tag = "Enemigo"
        PictureBox48.Tag = "Enemigo"
        PictureBox49.Tag = "Enemigo"

        PictureBox50.Tag = "Pinchos"
        PictureBox51.Tag = "Pinchos"
        PictureBox52.Tag = "Pinchos"
        PictureBox53.Tag = "Pinchos"
        PictureBox54.Tag = "Pinchos"
        PictureBox55.Tag = "Pinchos"
        PictureBox56.Tag = "Pinchos"
        PictureBox57.Tag = "Pinchos"

        Elevador.Tag = "Final"

        For Each pb As PictureBox In Me.Controls.OfType(Of PictureBox)()
            If pb.Tag IsNot Nothing AndAlso pb.Tag.ToString() = "Enemigo" Then
                enemyDirections(pb) = True
            End If
        Next

        Me.KeyPreview = True

        TimerEnemyMove.Interval = 50
        AddHandler TimerEnemyMove.Tick, AddressOf TimerEnemyMove_Tick
        TimerEnemyMove.Start()

        TimerPociones.Interval = 50
        AddHandler TimerPociones.Tick, AddressOf TimerPociones_Tick
        TimerPociones.Start()

        ' Inicializar las etiquetas de vidas y puntuación
        lblvidas.Text = "" & vidas
        lblpuntuacion.Text = "Puntuación: " & puntuacion
    End Sub

    Private Sub Jump()
        If Not jumping Then
            jumping = True
            jumpDistance = 0
            Timerjump.Start()
        End If
    End Sub

    Private Function IsOnPlatform() As Boolean
        For Each pb As PictureBox In Me.Controls.OfType(Of PictureBox)()
            If pb.Tag IsNot Nothing AndAlso pb.Tag.ToString() = "Plataforma" Then
                If Player1.Bounds.IntersectsWith(pb.Bounds) AndAlso Player1.Bottom <= pb.Top + GravitySpeed Then
                    Return True
                End If
            End If
        Next
        Return False
    End Function

    Private Sub TimerJump_Tick(sender As Object, e As EventArgs) Handles Timerjump.Tick
        If jumping Then
            Player1.Top -= JumpSpeed
            jumpDistance += JumpSpeed

            ' Si alcanza la altura máxima del salto o colisiona con un techo, detiene el salto
            If jumpDistance >= JumpHeight Or Player1.Top <= 0 Then
                jumping = False
                Timerjump.Stop()
                TimerGravity.Start()
            End If
        End If
    End Sub

    Private Sub TimerGravity_Tick(sender As Object, e As EventArgs) Handles TimerGravity.Tick
        ' Aplica la gravedad si el jugador no está en una plataforma y no está saltando
        If Not IsOnPlatform() AndAlso Not jumping Then
            Player1.Top += GravitySpeed
        End If

        ' Verifica si el jugador se cae al suelo del formulario
        If Player1.Top >= Me.ClientSize.Height - Player1.Height Then
            Player1.Location = initialPosition
            TimerGravity.Stop()
            jumpDistance = 0
            jumping = False
            PerderVida()
        End If

        ' Verifica si el jugador toca el objeto de victoria
        If pocionesRecolectadas = pocionesTotales Then
            For Each pb As PictureBox In Me.Controls.OfType(Of PictureBox)()
                If pb.Tag IsNot Nothing AndAlso pb.Tag.ToString() = "Final" Then
                    If Player1.Bounds.IntersectsWith(pb.Bounds) Then
                        If Not victoriaFormMostrado Then
                            Dim victoriaForm As New Form4()
                            victoriaFormMostrado = True
                            Showexit()
                            Timerjump.Stop()
                            TimerGravity.Stop()
                            TimerMoveLeft.Stop()
                            TimerMoveRight.Stop()
                        End If
                        Exit Sub
                    End If
                End If
            Next
        End If

        ' Verifica colisión con enemigos
        For Each pb As PictureBox In Me.Controls.OfType(Of PictureBox)()
            If pb.Tag IsNot Nothing AndAlso pb.Tag.ToString() = "Enemigo" Then
                If Player1.Bounds.IntersectsWith(pb.Bounds) Then
                    If Player1.Top + Player1.Height <= pb.Top + GravitySpeed AndAlso Not jumping Then
                        Me.Controls.Remove(pb)
                        GanarPuntos(50) ' Ganar puntos por eliminar un enemigo
                        Exit For
                    Else
                        ' Reinicia al inicio si toca un enemigo por los lados
                        Player1.Location = initialPosition
                        TimerGravity.Stop()
                        jumpDistance = 0
                        jumping = False
                        PerderVida()
                        Exit For
                    End If
                End If
            End If
        Next

        ' Verifica colisión con cajas
        For Each pb As PictureBox In Me.Controls.OfType(Of PictureBox)()
            If pb.Tag IsNot Nothing AndAlso pb.Tag.ToString() = "Caja" Then
                If Player1.Bounds.IntersectsWith(pb.Bounds) Then
                    If Player1.Left + Player1.Width > pb.Left AndAlso TimerMoveRight.Enabled Then
                        TimerMoveRight.Stop()
                    End If
                    If Player1.Right > pb.Right AndAlso TimerMoveLeft.Enabled Then
                        TimerMoveLeft.Stop()
                    End If
                End If
            End If
        Next

        ' Verifica colisión con pinchos
        For Each pb As PictureBox In Me.Controls.OfType(Of PictureBox)()
            If pb.Tag IsNot Nothing AndAlso pb.Tag.ToString() = "Pinchos" Then
                If Player1.Bounds.IntersectsWith(pb.Bounds) Then
                    Player1.Location = initialPosition
                    TimerGravity.Stop()
                    jumpDistance = 0
                    jumping = False
                    PerderVida()
                    Exit For
                End If
            End If
        Next
    End Sub

    Private Sub TimerMoveLeft_Tick(sender As Object, e As EventArgs) Handles TimerMoveLeft.Tick
        Player1.Left -= MoveSpeed
    End Sub

    Private Sub TimerMoveRight_Tick(sender As Object, e As EventArgs) Handles TimerMoveRight.Tick
        Player1.Left += MoveSpeed
    End Sub

    Private Sub Form1_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        If e.KeyCode = Keys.Escape Then
            ShowMenu()
        End If
        If e.KeyCode = Keys.Space Then
            If Not jumping AndAlso (IsOnPlatform() OrElse Player1.Bottom >= Me.ClientSize.Height - Player1.Height) Then
                jumping = True
                jumpDistance = 0
                Timerjump.Start()
            End If
        ElseIf e.KeyCode = Keys.Left Then
            ' Inicia el movimiento hacia la izquierda
            TimerMoveLeft.Start()
        ElseIf e.KeyCode = Keys.Right Then
            ' Inicia el movimiento hacia la derecha
            TimerMoveRight.Start()
        End If
    End Sub

    ' Mostrar el formulario del menú
    Private Sub ShowMenu()
        ' Deshabilita los timers para que el juego se pause
        Timerjump.Stop()
        TimerGravity.Stop()
        TimerMoveLeft.Stop()
        TimerMoveRight.Stop()
    End Sub

    Private Sub Showexit()
        ' Deshabilita los timers para que el juego se pause
        Timerjump.Stop()
        TimerGravity.Stop()
        TimerMoveLeft.Stop()
        TimerMoveRight.Stop()

        ' Crear una instancia del formulario del menú
        Dim FormExit As New Form4()

        ' Manejar los eventos del formulario del menú
        ' Mostrar el menú como un cuadro de diálogo
        FormExit.ShowDialog()
    End Sub

    ' Evento cuando se cierra el menú
    Private Sub MenuClosed(sender As Object, e As EventArgs)
        ' Habilita los timers si el menú se cierra
        If jumping Then
            Timerjump.Start()
        Else
            TimerGravity.Start()
        End If
    End Sub

    ' Evento cuando se reinicia el juego
    Private Sub GameRestarted(sender As Object, e As EventArgs)
        Player1.Location = initialPosition
        jumping = False
        jumpDistance = 0
        Timerjump.Stop()
        TimerGravity.Stop()
        TimerMoveLeft.Stop()
        TimerMoveRight.Stop()
        TimerGravity.Start()
    End Sub

    Private Sub Form1_KeyUp(sender As Object, e As KeyEventArgs) Handles MyBase.KeyUp
        If e.KeyCode = Keys.Left Then
            ' Detiene el movimiento hacia la izquierda cuando se suelta la tecla A
            TimerMoveLeft.Stop()
        ElseIf e.KeyCode = Keys.Right Then
            ' Detiene el movimiento hacia la derecha cuando se suelta la tecla D
            TimerMoveRight.Stop()
        End If
    End Sub

    Private Sub TimerEnemyMove_Tick(sender As Object, e As EventArgs)
        For Each enemy As PictureBox In Me.Controls.OfType(Of PictureBox)()
            If enemy.Tag IsNot Nothing AndAlso enemy.Tag.ToString() = "Enemigo" Then
                Dim direction As Boolean = enemyDirections(enemy)
                Dim moveDistance As Integer = If(direction, EnemyMoveSpeed, -EnemyMoveSpeed)
                enemy.Left += moveDistance

                ' Verificar los límites de la plataforma para invertir la dirección
                Dim onPlatform As Boolean = False
                For Each platform As PictureBox In Me.Controls.OfType(Of PictureBox)()
                    If platform.Tag IsNot Nothing AndAlso platform.Tag.ToString() = "Plataforma" Then
                        If enemy.Bounds.IntersectsWith(platform.Bounds) Then
                            If direction AndAlso enemy.Right >= platform.Right Then
                                enemyDirections(enemy) = False
                            ElseIf Not direction AndAlso enemy.Left <= platform.Left Then
                                enemyDirections(enemy) = True
                            End If
                            onPlatform = True
                            Exit For
                        End If
                    End If
                Next
                If Not onPlatform Then
                    enemyDirections(enemy) = Not direction
                End If
            End If
        Next
    End Sub

    Private Sub TimerPociones_Tick(sender As Object, e As EventArgs)
        ' Verifica colisión con pociones
        For Each pb As PictureBox In Me.Controls.OfType(Of PictureBox)()
            If pb.Tag IsNot Nothing AndAlso pb.Tag.ToString() = "Pocion" Then
                If Player1.Bounds.IntersectsWith(pb.Bounds) Then
                    Me.Controls.Remove(pb)
                    pocionesRecolectadas += 1
                    puntuacion += 10 ' Ganar puntos por recolectar una poción
                    lblpuntuacion.Text = "Puntuación: " & puntuacion
                    lblPocionesRecolectadas.Text = "x " & pocionesRecolectadas
                    Exit For
                End If
            End If
        Next

        ' Verifica si se han recolectado todas las pociones
        If pocionesRecolectadas = pocionesTotales Then
            ' Habilita el movimiento hacia la puerta de la victoria
            For Each pb As PictureBox In Me.Controls.OfType(Of PictureBox)()
                If pb.Tag IsNot Nothing AndAlso pb.Tag.ToString() = "Final" Then
                    pb.Enabled = True
                    pb.Visible = True
                    Exit For
                End If
            Next
        End If
    End Sub

    Private Sub GanarPuntos(puntos As Integer)
        puntuacion += puntos
        lblpuntuacion.Text = "Puntuación: " & puntuacion
    End Sub

    Private Sub PerderVida()
        vidas -= 1
        lblvidas.Text = "" & vidas
        If vidas <= 0 Then
            ' Mostrar mensaje de fin del juego
            MessageBox.Show("Fin del juego. ¡Has perdido todas tus vidas!")
            ' Detener todos los timers
            Timerjump.Stop()
            TimerGravity.Stop()
            TimerMoveLeft.Stop()
            TimerMoveRight.Stop()
            TimerEnemyMove.Stop()
            TimerPociones.Stop()
        End If
    End Sub

    Private Sub Form3_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        TimerGravity.Start()
    End Sub
End Class