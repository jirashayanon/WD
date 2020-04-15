Imports System.Runtime.InteropServices

Imports System.Reflection
Imports System.Diagnostics
Imports System.Windows.Forms
Imports System.Xml

Imports WDConnect
Imports WDConnect.Common
Imports WDConnect.Application
Imports MetrologyHost
Imports MetrologyHost.Classes
Imports MetrologyHost.Classes.Communication.WDConnect
Imports MetrologyHost.Classes.Utilities

Imports WDHelpers.ConfigHelper


'***************************************************************************
<Guid("C02B480F-F2A8-4DBF-B999-E89FD99908BD"), InterfaceType(ComInterfaceType.InterfaceIsIDispatch)>
Public Interface IWDConnectWrapperLibClass
    <DispId(1)> Function GetFileVersion() As String
    <DispId(2)> Function Instance() As WDConnectWrapperLibClass
    <DispId(3)> ReadOnly Property Online() As Boolean
    <DispId(4)> Sub Init()
    <DispId(5)> Sub SendAreYouThere()
    <DispId(6)> Property ProcessState() As PROCESS_STATE
    <DispId(7)> Sub SendProcessState()
    <DispId(8)> Sub ClearSECMessageBuffer()
    <DispId(9)> ReadOnly Property PrimaryInMessage As String
    <DispId(10)> ReadOnly Property SecondaryInMessage As String
    <DispId(11)> ReadOnly Property HostErrorMessage As String

    <DispId(12)> Sub GetTrayInfo(ByVal strBarcode As String)
    <DispId(13)> Function GetTrayInfoFromXML(ByVal strTrayInfoXML As String) As TrayInfo

    <DispId(14)> Sub UpdateTrayProcess(ByVal procTray As ProcessTray)
    <DispId(15)> Sub SendTrayStatus(ByVal strTrayBarcode As String, ByVal strLotNumber As String, ByVal nStatus As TRAY_STATUS)
    <DispId(16)> Sub RequestUnloadTray(ByVal strTrayBarcode As String, ByVal strLotNumber As String)
    <DispId(17)> Sub RequestCancelProcessedLot(ByVal strLotNumber As String)

    <DispId(18)> Sub RequestRemoveRemainTray(ByVal strLotNumber As String,
                                       ByVal strTrayBarcode1 As String,
                                       ByVal strTrayBarcode2 As String,
                                       ByVal strTrayBarcode3 As String,
                                       ByVal strTrayBarcode4 As String,
                                       ByVal strTrayBarcode5 As String,
                                       ByVal strTrayBarcode6 As String,
                                       ByVal strTrayBarcode7 As String,
                                       ByVal strTrayBarcode8 As String,
                                       ByVal strTrayBarcode9 As String,
                                       ByVal strTrayBarcode10 As String,
                                       ByVal strTrayBarcode11 As String,
                                       ByVal strTrayBarcode12 As String)

    <DispId(19)> Sub ClosePack(ByVal strTrayBarcode1 As String,
                     ByVal strTrayBarcode2 As String,
                     ByVal strTrayBarcode3 As String,
                     ByVal strTrayBarcode4 As String,
                     ByVal strTrayBarcode5 As String,
                     ByVal strTrayBarcode6 As String,
                     ByVal strTrayBarcode7 As String,
                     ByVal strTrayBarcode8 As String,
                     ByVal strTrayBarcode9 As String,
                     ByVal strTrayBarcode10 As String,
                     ByVal strTrayBarcode11 As String,
                     ByVal strTrayBarcode12 As String)

    <DispId(20)> Function GetCOMMACKFromXML(ByVal xmlGetCOMMACK As String) As Integer
    <DispId(21)> Function GetRequestUnloadTrayResultFromXML(ByVal xmlGetRequestUnloadTrayResult As String) As Boolean

    <DispId(22)> Property ControlState() As CONTROL_STATE
    <DispId(23)> Sub SendControlState()

    <DispId(24)> Function GetALIDFromXML(ByVal xmlGetALID As String) As Integer
    <DispId(25)> Sub SendAlarmReport(ByVal alarm As Alarm)

    <DispId(26)> Sub SendPkgSetting(ByVal pkgObj As PkgSetting)

    <DispId(27)> ReadOnly Property T3Timeout() As Integer
    <DispId(28)> ReadOnly Property T5Timeout() As Integer
    <DispId(29)> ReadOnly Property T6Timeout() As Integer
    <DispId(30)> ReadOnly Property T7Timeout() As Integer
    <DispId(31)> ReadOnly Property T8Timeout() As Integer
    <DispId(32)> ReadOnly Property MachineName As String

End Interface


<Guid("9F5E4633-8EF0-499D-A7A9-CB2C251223A2"), ClassInterface(ClassInterfaceType.None), ProgId("WDConnectWrapperLib")>
Public Class WDConnectWrapperLibClass
    Implements IWDConnectWrapperLibClass

    Public ReadOnly Log As log4net.ILog
    Private Shared _instance As WDConnectWrapperLibClass

    Private _equipment As EquipmentController
    Private _setting As BuilderSetting
    Private _online As Boolean
    '***************************************************************************
    'ctor
    Public Sub New()
        Log = log4net.LogManager.GetLogger("MainWindow")

        Dim configPath As String = System.Windows.Forms.Application.StartupPath
        Dim configFile As System.IO.FileInfo
        configFile = New System.IO.FileInfo(configPath + "\WDLog.config")

        log4net.Config.XmlConfigurator.Configure(configFile)
        Log.Info("WDConnectWrapperLibClass ctor")

        _equipment = New EquipmentController()

        _setting = BuilderSetting.Instance()
        FileUtilities.EnsureDirectory(_setting.RoothPath)
        FileUtilities.EnsureDirectory(_setting.ModelsPath)
        FileUtilities.EnsureDirectory(_setting.SettingPath)
        FileUtilities.EnsureDirectory(_setting.MessageFilePath)

    End Sub

    'dtor
    Protected Overrides Sub Finalize()

    End Sub



    Private Sub RegisterHost()
        AddHandler _equipment.WDConnectPrimaryIn, AddressOf equipmentController_SECsPrimaryIn
        AddHandler _equipment.WDConnectSecondaryIn, AddressOf equipmentController_SECsSecondaryIn
        AddHandler _equipment.WDConnectHostError, AddressOf equipmentController_SECsHostError
    End Sub



    Private Sub UnregisterHost()
        If (IsDBNull(_equipment)) Then
            Return
        End If

        _equipment.ClearEventInvocations("WDConnectPrimaryIn")
        _equipment.ClearEventInvocations("WDConnectSecondaryIn")
        _equipment.ClearEventInvocations("WDConnectHostError")
    End Sub


    Private _transactionID As Integer
    Private _primaryInSECMessage As String
    Private Sub equipmentController_SECsPrimaryIn(ByVal sender As Object, ByVal e As SECsPrimaryInEventArgs)
        Log.Info("equipmentController_SECsPrimaryIn: " + e.Transaction.XMLText)
        _primaryInSECMessage = e.Transaction.XMLText

        _transactionID = e.Transaction.Id
        If (IsNothing(e.Transaction.Primary)) Then
            'primary is null

            If (IsNothing(e.Transaction.Secondary)) Then
                'secondary is null
            Else
                Log.Info("equipmentController_SECsPrimaryIn, Primary null, Secondary not null")
                Log.Info("secondary not null: " + e.Transaction.XMLText)
            End If

        Else
            Select Case e.Transaction.Primary.CommandID

                Case "Connected"
                    Dim trans As SCITransaction = New SCITransaction
                    trans.MessageType = MessageType.Primary
                    trans.Name = "Event:FinishedInitialize"
                    trans.NeedReply = False

                    Dim msg As SCIMessage = New SCIMessage
                    msg.CommandID = "Event:FinishedInitialize"
                    trans.Primary = msg

                    SendMessage(trans)


                Case "AreYouThere"

                    If (Online) Then
                        SendAreYouThereAck(e.Transaction)

                    Else
                        RejectCommandWhileOffline(e.Transaction)
                    End If


                Case "OnlineRequest"

                    If (Not Online) Then
                        'Online = True
                        _equipment._controlState = CONTROL_STATE.ONLINE_REMOTE
                    End If

                    OnlineRequestAck(e.Transaction)


                Case "OfflineRequest"

                    If (Online) Then
                        'Online = False
                        _equipment.ControlState = CONTROL_STATE.OFFLINE
                    End If

                    OfflineRequestAck(e.Transaction)


                Case "RequestEquipmentStatus"   'obsolete

                    If (Online) Then
                        'SendStatus(_equipment.Status)      'obsolete
                        RequestProcessStateAck(e.Transaction)

                    Else
                        RejectCommandWhileOffline(e.Transaction)
                    End If


                Case "RequestEquipmentState"    'obsolete

                    If (Online) Then
                        'SendStatus(_equipment.Status)      'obsolete
                        RequestProcessStateAck(e.Transaction)

                    Else
                        RejectCommandWhileOffline(e.Transaction)
                    End If


                Case "RequestProcessState"

                    If (Online) Then
                        RequestProcessStateAck(e.Transaction)

                    Else
                        RejectCommandWhileOffline(e.Transaction)
                    End If


                Case "RequestControlState"

                    RequestControlStateAck(e.Transaction)


                Case "AlarmReportSend"

                    If (Online) Then
                        SendAlarmReportAck(e.Transaction)

                    Else
                        RejectCommandWhileOffline(e.Transaction)
                    End If


                Case Else
                    'default

            End Select
        End If

    End Sub


    Private _secondaryInSECMessage As String
    Private Sub equipmentController_SECsSecondaryIn(ByVal sender As Object, ByVal e As SECsSecondaryInEventArgs)
        Log.Info("equipmentController_SECsSecondaryIn: " + e.Transaction.XMLText)
        _secondaryInSECMessage = e.Transaction.XMLText

        _transactionID = e.Transaction.Id
        If (Not IsDBNull(e.Transaction.Secondary)) Then
            Select Case e.Transaction.Secondary.CommandID

                Case "AreYouThereAck"
                    'done nothing

                Case "SendStatusAck"
                    'done nothing

                Case "GetTrayInfoAck"
                    'done nothing

                Case "SendTrayStatusAck"
                    'need to check COMMACK

                Case "RequestRemoveRemainTrayAck"
                    'need to check COMMACK

                Case "RequestUnloadTrayAck"
                    'need to check COMMACK
                    'need to check CanUnload flag

                Case "RequestCancelProcessedLotAck"
                    'need to check COMMACK
                    'need to check list of trays

                Case "UpdateTrayProcessAck"
                    'done nothing

                Case "ClosePackAck"
                    'need to check COMMACK


                Case Else
                    'none

            End Select


        Else

            If (Not IsDBNull(e.Transaction.Primary)) Then
                Log.Info("equipmentController_SECsSecondaryIn, Secondary null, Primary not null")
                Log.Info("primary not null: " + e.Transaction.XMLText)
            End If

        End If
    End Sub


    Private _hostErrorMessage As String = ""
    Private Sub equipmentController_SECsHostError(ByVal sender As Object, ByVal e As SECsHostErrorEventArgs)
        Log.Info("equipmentController_SECsHostError: " + e.Message)
        _hostErrorMessage = e.Message
    End Sub


    Public Sub SendMessage(ByVal trans As SCITransaction)
        Log.Info("SendMessage: " + trans.XMLText)
        _equipment.SendMessage(trans)
        Log.Info("SendMessage, return")
    End Sub


    Public Sub ConnectHost()
        _equipment.Connect()
    End Sub


    Public Sub DisconnectHost()
        _equipment.Disconnect()
    End Sub



    '***************************************************************************
#Region "DispID1" '<DispId(1)> Function GetFileVersion() As String
    Public Function GetFileVersion() As String Implements IWDConnectWrapperLibClass.GetFileVersion
        Log.Info("IWDConnectWrapperLibClass.GetFileVersion")

        Dim asm As System.Reflection.Assembly
        asm = System.Reflection.Assembly.GetExecutingAssembly()

        Dim fvi As FileVersionInfo
        fvi = FileVersionInfo.GetVersionInfo(asm.Location)

        Return fvi.FileVersion
    End Function

#End Region


    '***************************************************************************
#Region "DispID2" '<DispId(2)> Function Instance() As WDConnectWrapperLibClass
    Public Function Instance() As WDConnectWrapperLibClass Implements IWDConnectWrapperLibClass.Instance
        If _instance Is Nothing Then
            Log.Info("WDConnectWrapperLibClass instantiated")
            _instance = New WDConnectWrapperLibClass
        End If

        Return _instance
    End Function

#End Region


    '***************************************************************************
#Region "DispID3" '<DispId(3)> ReadOnly Property Online() As Boolean
    Public ReadOnly Property Online() As Boolean Implements IWDConnectWrapperLibClass.Online
        Get
            Dim bOnline As Boolean = False
            If ((_equipment._controlState And 4) = 4) Then
                bOnline = True
            End If

            Return bOnline
        End Get

        'Set(ByVal value As Boolean)
        '    _online = value
        'End Set
    End Property

#End Region


    '***************************************************************************
#Region "DispID4" '<DispId(4)> Sub Init()
    Public Sub Init() Implements IWDConnectWrapperLibClass.Init
        Log.Info("WDConnectWrapperLibClass.Init")

        Dim executePath As String = System.Windows.Forms.Application.StartupPath
        Dim toolModelPath As String = executePath + "\Local\ToolModel"

        _equipment.Initialize(toolModelPath + "\ASAM.xml")

        UnregisterHost()
        RegisterHost()

        _equipment.Connect()
    End Sub

#End Region


    '***************************************************************************
#Region "DispID5"        '<DispId(5)> Sub SendAreYouThere()
    Public Sub SendAreYouThere() Implements IWDConnectWrapperLibClass.SendAreYouThere
        Dim primMsg As SCIMessage = New SCIMessage
        primMsg.CommandID = "AreYouThere"
        primMsg.Item = New SCIItem
        primMsg.Item.Format = SCIFormat.List
        primMsg.Item.Items = New SCIItemCollection
        primMsg.Item.Items.Add(New SCIItem With {.Format = SCIFormat.String, .Name = "SoftwareRevision", .Value = _equipment.SoftwareRevision})

        Dim trans As SCITransaction = New SCITransaction With
        {
            .DeviceId = _equipment.DeviceID,
            .MessageType = MessageType.Primary,
            .Id = _transactionID + 1,
            .Name = "AreYouThere",
            .NeedReply = True,
            .Primary = primMsg,
            .Secondary = Nothing
        }

        SendMessage(trans)
    End Sub


    Public Sub SendAreYouThereAck()
        Dim scndMsg As SCIMessage = New SCIMessage
        scndMsg.CommandID = "AreYouThereAck"
        scndMsg.Item = New SCIItem
        scndMsg.Item.Format = SCIFormat.List
        scndMsg.Item.Items = New SCIItemCollection

        scndMsg.Item.Items.Add(New SCIItem With {.Format = SCIFormat.String, .Name = "SoftwareRevision", .Value = _equipment.SoftwareRevision})

        Dim trans As SCITransaction = New SCITransaction With
        {
            .DeviceId = _equipment.DeviceID,
            .MessageType = MessageType.Secondary,
            .Id = _transactionID,
            .Name = "AreYouThereAck",
            .NeedReply = False,
            .Primary = Nothing,
        .Secondary = scndMsg
        }

        SendMessage(trans)
    End Sub



    Public Sub SendAreYouThereAck(ByVal priTrans As SCITransaction)
        Dim scndMsg As SCIMessage = New SCIMessage
        scndMsg.CommandID = "AreYouThereAck"
        scndMsg.Item = New SCIItem
        scndMsg.Item.Format = SCIFormat.List
        scndMsg.Item.Items = New SCIItemCollection

        scndMsg.Item.Items.Add(New SCIItem With {.Format = SCIFormat.String, .Name = "SoftwareRevision", .Value = _equipment.SoftwareRevision})

        Dim trans As SCITransaction = New SCITransaction With
        {
            .DeviceId = _equipment.DeviceID,
            .MessageType = MessageType.Secondary,
            .Id = priTrans.Id,
            .Name = "AreYouThereAck",
            .NeedReply = False,
            .Primary = priTrans.Primary,
            .Secondary = scndMsg
        }

        SendMessage(trans)
    End Sub

#End Region


    '***************************************************************************
#Region "DispID6" '<DispId(6)> Property ProcessState() As PROCESS_STATE
    Public Property ProcessState() As PROCESS_STATE Implements IWDConnectWrapperLibClass.ProcessState
        Get
            Return _equipment.ProcessState
        End Get
        Set(ByVal value As PROCESS_STATE)
            _equipment.ProcessState = value
        End Set
    End Property

#End Region


    '***************************************************************************
#Region "DispID7" '<DispId(7)> Sub SendProcessState()
    Public Sub SendProcessState() Implements IWDConnectWrapperLibClass.SendProcessState
        Log.Info("WDConnectWrapperLibClass.SendProcessState")
        SendProcessState(_equipment.ProcessState)
    End Sub

#End Region


    '***************************************************************************
#Region "DispID8" '<DispId(8)> Sub ClearSECMessageBuffer()
    Public Sub ClearSECMessageBuffer() Implements IWDConnectWrapperLibClass.ClearSECMessageBuffer
        _primaryInSECMessage = ""
        _secondaryInSECMessage = ""
        _hostErrorMessage = ""
    End Sub

#End Region


    '***************************************************************************
#Region "DispID9" '<DispId(9)> ReadOnly Property PrimaryInMessage
    Public ReadOnly Property PrimaryInMessage As String Implements IWDConnectWrapperLibClass.PrimaryInMessage
        Get
            Return _primaryInSECMessage
        End Get
    End Property

#End Region


    '***************************************************************************
#Region "DispID10" '<DispId(10)> ReadOnly Property SecondaryInMessage
    Public ReadOnly Property SecondaryInMessage As String Implements IWDConnectWrapperLibClass.SecondaryInMessage
        Get
            Return _secondaryInSECMessage
        End Get
    End Property

#End Region


    '***************************************************************************
#Region "DispID11" '<DispId(11)> ReadOnly Property HostErrorMessage
    Public ReadOnly Property HostErrorMessage As String Implements IWDConnectWrapperLibClass.HostErrorMessage
        Get
            Return _hostErrorMessage
        End Get
    End Property

#End Region


    '***************************************************************************
#Region "DispID12" '<DispId(12)> Sub GetTrayInfo(ByVal strBarcode As String)
    Public Sub GetTrayInfo(ByVal strTrayBarcode As String) Implements IWDConnectWrapperLibClass.GetTrayInfo
        Dim primMsg As SCIMessage = New SCIMessage
        primMsg.CommandID = "GetTrayInfo"
        primMsg.Item = New SCIItem
        primMsg.Item.Format = SCIFormat.List
        primMsg.Item.Items = New SCIItemCollection

        primMsg.Item.Items.Add(New SCIItem With {.Format = SCIFormat.String, .Name = "MachineName", .Value = _equipment.MachineName})
        primMsg.Item.Items.Add(New SCIItem With {.Format = SCIFormat.String, .Name = "TrayBarcode", .Value = strTrayBarcode})

        Dim trans As SCITransaction = New SCITransaction With
        {
            .DeviceId = _equipment.DeviceID,
            .MessageType = MessageType.Primary,
            .Id = _transactionID + 1,
            .Name = "GetTrayInfo",
            .NeedReply = True,
            .Primary = primMsg,
            .Secondary = Nothing
        }

        SendMessage(trans)
    End Sub

#End Region


    '***************************************************************************
#Region "DispID13" '<DispId(13)> Function GetTrayInfoFromXML(ByVal strTrayInfoXML As String) As TrayInfo
    Public Function GetTrayInfoFromXML(ByVal xmlGetTrayInfo As String) As TrayInfo Implements IWDConnectWrapperLibClass.GetTrayInfoFromXML
        Dim returnedTrayInfo As TrayInfo = New TrayInfo()

        If (xmlGetTrayInfo.Length < 1) Then
            Return returnedTrayInfo
        End If


        Dim gettrayinfoXMLDoc As XmlDocument = New XmlDocument()
        gettrayinfoXMLDoc.LoadXml(xmlGetTrayInfo)

        Dim root_gettrayXMLDoc As XmlElement = gettrayinfoXMLDoc.DocumentElement
        Dim gettrayInfoNodeList As XmlNodeList = root_gettrayXMLDoc.SelectNodes("/SCITransaction/Secondary/Item/Items/SCIItem")

        If (gettrayInfoNodeList.Count > 0) Then
            For Each node As XmlNode In gettrayInfoNodeList

                Dim nameNodeList As XmlNodeList = node.SelectNodes("./Name")
                Dim valueNodeList As XmlNodeList = node.SelectNodes("./Value")

                For Each nameChildNode As XmlNode In nameNodeList
                    If (nameChildNode.InnerText = "TrayBarcode") Then

                        For Each valueChildNode As XmlNode In valueNodeList
                            returnedTrayInfo.TrayBarcode = valueChildNode.InnerText
                        Next

                    End If
                Next


                For Each nameChildNode As XmlNode In nameNodeList
                    If (nameChildNode.InnerText = "IsInProcessLot") Then

                        For Each valueChildNode As XmlNode In valueNodeList
                            returnedTrayInfo.IsInProcessLot = Convert.ToBoolean(valueChildNode.InnerText)
                        Next

                    End If
                Next


                For Each nameChildNode As XmlNode In nameNodeList
                    If (nameChildNode.InnerText = "LotNumber") Then

                        For Each valueChildNode As XmlNode In valueNodeList
                            returnedTrayInfo.LotNumber = valueChildNode.InnerText
                        Next

                    End If
                Next


                For Each nameChildNode As XmlNode In nameNodeList
                    If (nameChildNode.InnerText = "TrayBarcode#1") Then

                        For Each valueChildNode As XmlNode In valueNodeList
                            returnedTrayInfo.TrayBarcodePos1 = valueChildNode.InnerText
                        Next

                    End If
                Next


                For Each nameChildNode As XmlNode In nameNodeList
                    If (nameChildNode.InnerText = "TrayBarcode#2") Then

                        For Each valueChildNode As XmlNode In valueNodeList
                            returnedTrayInfo.TrayBarcodePos2 = valueChildNode.InnerText
                        Next

                    End If
                Next


                For Each nameChildNode As XmlNode In nameNodeList
                    If (nameChildNode.InnerText = "TrayBarcode#3") Then

                        For Each valueChildNode As XmlNode In valueNodeList
                            returnedTrayInfo.TrayBarcodePos3 = valueChildNode.InnerText
                        Next

                    End If
                Next


                For Each nameChildNode As XmlNode In nameNodeList
                    If (nameChildNode.InnerText = "TrayBarcode#4") Then

                        For Each valueChildNode As XmlNode In valueNodeList
                            returnedTrayInfo.TrayBarcodePos4 = valueChildNode.InnerText
                        Next

                    End If
                Next


                For Each nameChildNode As XmlNode In nameNodeList
                    If (nameChildNode.InnerText = "TrayBarcode#5") Then

                        For Each valueChildNode As XmlNode In valueNodeList
                            returnedTrayInfo.TrayBarcodePos5 = valueChildNode.InnerText
                        Next

                    End If
                Next


                For Each nameChildNode As XmlNode In nameNodeList
                    If (nameChildNode.InnerText = "TrayBarcode#6") Then

                        For Each valueChildNode As XmlNode In valueNodeList
                            returnedTrayInfo.TrayBarcodePos6 = valueChildNode.InnerText
                        Next

                    End If
                Next


                For Each nameChildNode As XmlNode In nameNodeList
                    If (nameChildNode.InnerText = "TrayBarcode#7") Then

                        For Each valueChildNode As XmlNode In valueNodeList
                            returnedTrayInfo.TrayBarcodePos7 = valueChildNode.InnerText
                        Next

                    End If
                Next


                For Each nameChildNode As XmlNode In nameNodeList
                    If (nameChildNode.InnerText = "TrayBarcode#8") Then

                        For Each valueChildNode As XmlNode In valueNodeList
                            returnedTrayInfo.TrayBarcodePos8 = valueChildNode.InnerText
                        Next

                    End If
                Next


                For Each nameChildNode As XmlNode In nameNodeList
                    If (nameChildNode.InnerText = "TrayBarcode#9") Then

                        For Each valueChildNode As XmlNode In valueNodeList
                            returnedTrayInfo.TrayBarcodePos9 = valueChildNode.InnerText
                        Next

                    End If
                Next


                For Each nameChildNode As XmlNode In nameNodeList
                    If (nameChildNode.InnerText = "TrayBarcode#10") Then

                        For Each valueChildNode As XmlNode In valueNodeList
                            returnedTrayInfo.TrayBarcodePos10 = valueChildNode.InnerText
                        Next

                    End If
                Next


                For Each nameChildNode As XmlNode In nameNodeList
                    If (nameChildNode.InnerText = "TrayBarcode#11") Then

                        For Each valueChildNode As XmlNode In valueNodeList
                            returnedTrayInfo.TrayBarcodePos11 = valueChildNode.InnerText
                        Next

                    End If
                Next


                For Each nameChildNode As XmlNode In nameNodeList
                    If (nameChildNode.InnerText = "TrayBarcode#12") Then

                        For Each valueChildNode As XmlNode In valueNodeList
                            returnedTrayInfo.TrayBarcodePos12 = valueChildNode.InnerText
                        Next

                    End If
                Next


                For Each nameChildNode As XmlNode In nameNodeList
                    If (nameChildNode.InnerText = "LotStatus") Then

                        For Each valueChildNode As XmlNode In valueNodeList
                            returnedTrayInfo.LotStatus = Convert.ToInt32(valueChildNode.InnerText, 10)
                        Next

                    End If
                Next


                For Each nameChildNode As XmlNode In nameNodeList
                    If (nameChildNode.InnerText = "NetworkSpec") Then

                        For Each valueChildNode As XmlNode In valueNodeList
                            returnedTrayInfo.NetworkSpec = valueChildNode.InnerText
                        Next

                    End If
                Next


                For Each nameChildNode As XmlNode In nameNodeList
                    If (nameChildNode.InnerText = "ProgramName") Then

                        For Each valueChildNode As XmlNode In valueNodeList
                            returnedTrayInfo.ProgName = valueChildNode.InnerText
                        Next

                    End If
                Next




                For Each nameChildNode As XmlNode In nameNodeList
                    If (nameChildNode.InnerText = "Product") Then

                        For Each valueChildNode As XmlNode In valueNodeList
                            returnedTrayInfo.Product = valueChildNode.InnerText
                        Next

                    End If
                Next


                For Each nameChildNode As XmlNode In nameNodeList
                    If (nameChildNode.InnerText = "Lot3250") Then

                        For Each valueChildNode As XmlNode In valueNodeList
                            returnedTrayInfo.Lot3250 = valueChildNode.InnerText
                        Next

                    End If
                Next


                For Each nameChildNode As XmlNode In nameNodeList
                    If (nameChildNode.InnerText = "OperationName") Then

                        For Each valueChildNode As XmlNode In valueNodeList
                            returnedTrayInfo.OperationName = valueChildNode.InnerText
                        Next

                    End If
                Next


                For Each nameChildNode As XmlNode In nameNodeList
                    If (nameChildNode.InnerText = "HGAPN") Then

                        For Each valueChildNode As XmlNode In valueNodeList
                            returnedTrayInfo.HGAPN = valueChildNode.InnerText
                        Next

                    End If
                Next


                For Each nameChildNode As XmlNode In nameNodeList
                    If (nameChildNode.InnerText = "QtyIn") Then

                        For Each valueChildNode As XmlNode In valueNodeList
                            returnedTrayInfo.QtyIn = Convert.ToInt32(valueChildNode.InnerText, 10)
                        Next

                    End If
                Next


                For Each nameChildNode As XmlNode In nameNodeList
                    If (nameChildNode.InnerText = "STR") Then

                        For Each valueChildNode As XmlNode In valueNodeList
                            returnedTrayInfo.STR = valueChildNode.InnerText
                        Next

                    End If
                Next


                For Each nameChildNode As XmlNode In nameNodeList
                    If (nameChildNode.InnerText = "Line") Then

                        For Each valueChildNode As XmlNode In valueNodeList
                            returnedTrayInfo.Line = valueChildNode.InnerText
                        Next

                    End If
                Next


                For Each nameChildNode As XmlNode In nameNodeList
                    If (nameChildNode.InnerText = "Suspension") Then

                        For Each valueChildNode As XmlNode In valueNodeList
                            returnedTrayInfo.Suspension = valueChildNode.InnerText
                        Next

                    End If
                Next


                For Each nameChildNode As XmlNode In nameNodeList
                    If (nameChildNode.InnerText = "Type") Then

                        For Each valueChildNode As XmlNode In valueNodeList
                            returnedTrayInfo.Type = valueChildNode.InnerText
                        Next

                    End If
                Next


                For Each nameChildNode As XmlNode In nameNodeList
                    If (nameChildNode.InnerText = "SuspInv") Then

                        For Each valueChildNode As XmlNode In valueNodeList
                            returnedTrayInfo.SuspInv = valueChildNode.InnerText
                        Next

                    End If
                Next


                For Each nameChildNode As XmlNode In nameNodeList
                    If (nameChildNode.InnerText = "SuspBatch") Then

                        For Each valueChildNode As XmlNode In valueNodeList
                            returnedTrayInfo.SuspBatch = valueChildNode.InnerText
                        Next

                    End If
                Next


                For Each nameChildNode As XmlNode In nameNodeList
                    If (nameChildNode.InnerText = "OSC") Then

                        For Each valueChildNode As XmlNode In valueNodeList
                            returnedTrayInfo.OSC = valueChildNode.InnerText
                        Next

                    End If
                Next


                For Each nameChildNode As XmlNode In nameNodeList
                    If (nameChildNode.InnerText = "LotUsage") Then

                        For Each valueChildNode As XmlNode In valueNodeList
                            returnedTrayInfo.LotUsage = valueChildNode.InnerText
                        Next

                    End If
                Next



            Next node
        End If


        Return returnedTrayInfo
    End Function

#End Region


    '***************************************************************************
#Region "DispID14" '<DispId(14)> Sub UpdateTrayProcess(ByVal procTray As ProcessTray)
    Public Sub UpdateTrayProcess(ByVal procTray As ProcessTray) Implements IWDConnectWrapperLibClass.UpdateTrayProcess
        Dim primMsg As SCIMessage = New SCIMessage
        primMsg.CommandID = "UpdateTrayProcess"
        primMsg.Item = New SCIItem
        primMsg.Item.Format = SCIFormat.List
        primMsg.Item.Items = New SCIItemCollection

        primMsg.Item.Items.Add(New SCIItem With {.Format = SCIFormat.String, .Name = "MachineName", .Value = _equipment.MachineName})
        primMsg.Item.Items.Add(New SCIItem With {.Format = SCIFormat.String, .Name = "TrayBarcode", .Value = procTray.TrayBarcode})
        primMsg.Item.Items.Add(New SCIItem With {.Format = SCIFormat.String, .Name = "LotNumber", .Value = procTray.LotNumber})

        primMsg.Item.Items.Add(New SCIItem With {.Format = SCIFormat.String, .Name = "HGAPartNumber", .Value = procTray.HGAPartNumber})
        primMsg.Item.Items.Add(New SCIItem With {.Format = SCIFormat.String, .Name = "ProgName", .Value = procTray.ProgName})
        primMsg.Item.Items.Add(New SCIItem With {.Format = SCIFormat.String, .Name = "HeadType", .Value = procTray.HeadType})

        primMsg.Item.Items.Add(New SCIItem With {.Format = SCIFormat.String, .Name = "AdjustFixtureID", .Value = procTray.AdjustFixtureID})
        primMsg.Item.Items.Add(New SCIItem With {.Format = SCIFormat.String, .Name = "NetSpecCode", .Value = procTray.NetSpecCode})
        primMsg.Item.Items.Add(New SCIItem With {.Format = SCIFormat.String, .Name = "ReadType", .Value = procTray.ReadType})
        primMsg.Item.Items.Add(New SCIItem With {.Format = SCIFormat.String, .Name = "SuspLot", .Value = procTray.SuspLot})
        primMsg.Item.Items.Add(New SCIItem With {.Format = SCIFormat.String, .Name = "STR", .Value = procTray.STR})
        primMsg.Item.Items.Add(New SCIItem With {.Format = SCIFormat.String, .Name = "CoverTrayBarcode", .Value = procTray.CoverTrayBarcode})
        primMsg.Item.Items.Add(New SCIItem With {.Format = SCIFormat.String, .Name = "CoverTrayMitecs", .Value = procTray.CoverTrayMitecs})
        primMsg.Item.Items.Add(New SCIItem With {.Format = SCIFormat.String, .Name = "LaserADJPowerTopBot", .Value = procTray.LaserADJPowerTopBot})

        primMsg.Item.Items.Add(New SCIItem With {.Format = SCIFormat.String, .Name = "LineAssyMC", .Value = procTray.LineAssyMC})
        primMsg.Item.Items.Add(New SCIItem With {.Format = SCIFormat.String, .Name = "OperatorID", .Value = procTray.OperatorID})



        '***************************
        Try
            For i As Integer = 0 To 59

                If (procTray.GetHga(i).Nest.Length < 1) And (procTray.GetHga(i).PositionOnNest = 0) Then
                    Continue For 'skip the empty position, having no HGA on tray
                End If

                Log.Info("UpdateTrayProcess, iteration#:" + i.ToString())

                'hga item
                Dim hgaItem As SCIItem = New SCIItem
                hgaItem.Format = SCIFormat.List
                hgaItem.Name = "RawProcessedDataHGA" + (i + 1).ToString
                hgaItem.Items = New SCIItemCollection

                hgaItem.Items.Add(New SCIItem With {.Format = SCIFormat.String, .Name = "Nest", .Value = procTray.GetHga(i).Nest})
                hgaItem.Items.Add(New SCIItem With {.Format = SCIFormat.Integer, .Name = "PositionOnNest", .Value = procTray.GetHga(i).PositionOnNest})
                hgaItem.Items.Add(New SCIItem With {.Format = SCIFormat.Integer, .Name = "PositionOnTray", .Value = procTray.GetHga(i).PositionOnTray})
                hgaItem.Items.Add(New SCIItem With {.Format = SCIFormat.Integer, .Name = "PartNo", .Value = procTray.GetHga(i).PartNo})


                hgaItem.Items.Add(New SCIItem With {.Format = SCIFormat.Float, .Name = "ZHeight", .Value = (procTray.GetHga(i).ZHeight)})
                hgaItem.Items.Add(New SCIItem With {.Format = SCIFormat.Float, .Name = "IPitch", .Value = (procTray.GetHga(i).IPitch)})
                hgaItem.Items.Add(New SCIItem With {.Format = SCIFormat.Float, .Name = "IRoll", .Value = (procTray.GetHga(i).IRoll)})
                hgaItem.Items.Add(New SCIItem With {.Format = SCIFormat.Float, .Name = "OPitch", .Value = (procTray.GetHga(i).OPitch)})
                hgaItem.Items.Add(New SCIItem With {.Format = SCIFormat.Float, .Name = "ORoll", .Value = (procTray.GetHga(i).ORoll)})
                hgaItem.Items.Add(New SCIItem With {.Format = SCIFormat.String, .Name = "OStatus", .Value = procTray.GetHga(i).OStatus})
                hgaItem.Items.Add(New SCIItem With {.Format = SCIFormat.Float, .Name = "Pitch", .Value = (procTray.GetHga(i).Pitch)})
                hgaItem.Items.Add(New SCIItem With {.Format = SCIFormat.Float, .Name = "Roll", .Value = (procTray.GetHga(i).Roll)})
                hgaItem.Items.Add(New SCIItem With {.Format = SCIFormat.String, .Name = "Status", .Value = procTray.GetHga(i).Status})


                hgaItem.Items.Add(New SCIItem With {.Format = SCIFormat.Float, .Name = "MeasuredPitchBFADJ", .Value = (procTray.GetHga(i).MeasuredPitchBFADJ)})
                hgaItem.Items.Add(New SCIItem With {.Format = SCIFormat.Float, .Name = "CompPitchBFADJ", .Value = (procTray.GetHga(i).CompPitchBFADJ)})
                hgaItem.Items.Add(New SCIItem With {.Format = SCIFormat.Float, .Name = "RollBFADJ", .Value = (procTray.GetHga(i).RollBFADJ)})
                hgaItem.Items.Add(New SCIItem With {.Format = SCIFormat.Float, .Name = "ZHeightBFADJ", .Value = (procTray.GetHga(i).ZHeightBFADJ)})

                hgaItem.Items.Add(New SCIItem With {.Format = SCIFormat.Float, .Name = "MeasuredPitchAFADJ", .Value = (procTray.GetHga(i).MeasuredPitchAFADJ)})
                hgaItem.Items.Add(New SCIItem With {.Format = SCIFormat.Float, .Name = "CompPitchAFADJ", .Value = (procTray.GetHga(i).CompPitchAFADJ)})
                hgaItem.Items.Add(New SCIItem With {.Format = SCIFormat.Float, .Name = "RollAFADJ", .Value = (procTray.GetHga(i).RollAFADJ)})
                hgaItem.Items.Add(New SCIItem With {.Format = SCIFormat.Float, .Name = "ZHeightAFADJ", .Value = (procTray.GetHga(i).ZHeightAFADJ)})

                hgaItem.Items.Add(New SCIItem With {.Format = SCIFormat.String, .Name = "AdjMarking", .Value = procTray.GetHga(i).AdjMarking})
                hgaItem.Items.Add(New SCIItem With {.Format = SCIFormat.Float, .Name = "VisionX", .Value = (procTray.GetHga(i).VisionX)})
                hgaItem.Items.Add(New SCIItem With {.Format = SCIFormat.Float, .Name = "VisionY", .Value = (procTray.GetHga(i).VisionY)})
                hgaItem.Items.Add(New SCIItem With {.Format = SCIFormat.Float, .Name = "VisionA", .Value = (procTray.GetHga(i).VisionA)})

                hgaItem.Items.Add(New SCIItem With {.Format = SCIFormat.String, .Name = "Date", .Value = procTray.GetHga(i).MachineDate})
                hgaItem.Items.Add(New SCIItem With {.Format = SCIFormat.String, .Name = "Time", .Value = procTray.GetHga(i).MachineTime})

                hgaItem.Items.Add(New SCIItem With {.Format = SCIFormat.String, .Name = "OCR", .Value = procTray.GetHga(i).OCR})
                hgaItem.Items.Add(New SCIItem With {.Format = SCIFormat.String, .Name = "SPCSamplingMarking", .Value = procTray.GetHga(i).SPCSamplingMarking})


                primMsg.Item.Items.Add(hgaItem)

            Next i
        Catch ex As Exception
            Log.Info("UpdateTrayProcess, exception:" + ex.Message)
        End Try

        '***************************

        Dim trans As SCITransaction = New SCITransaction With
        {
            .DeviceId = _equipment.DeviceID,
            .MessageType = MessageType.Primary,
            .Id = _transactionID + 1,
            .Name = "UpdateTrayProcess",
            .NeedReply = True,
            .Primary = primMsg,
            .Secondary = Nothing
        }

        SendMessage(trans)


    End Sub

#End Region


    '***************************************************************************
#Region "DispID15" '<DispId(15)> Sub SendTrayStatus(ByVal strTrayBarcode As String, ByVal strLotNumber As String, ByVal nStatus As TRAY_STATUS)
    Public Sub SendTrayStatus(ByVal strTrayBarcode As String, ByVal strLotNumber As String, ByVal nStatus As TRAY_STATUS) Implements IWDConnectWrapperLibClass.SendTrayStatus
        Dim primMsg As SCIMessage = New SCIMessage
        primMsg.CommandID = "SendTrayStatus"
        primMsg.Item = New SCIItem
        primMsg.Item.Format = SCIFormat.List
        primMsg.Item.Items = New SCIItemCollection

        primMsg.Item.Items.Add(New SCIItem With {.Format = SCIFormat.String, .Name = "MachineName", .Value = _equipment.MachineName})
        primMsg.Item.Items.Add(New SCIItem With {.Format = SCIFormat.String, .Name = "TrayBarcode", .Value = strTrayBarcode})
        primMsg.Item.Items.Add(New SCIItem With {.Format = SCIFormat.String, .Name = "LotNumber", .Value = strLotNumber})
        primMsg.Item.Items.Add(New SCIItem With {.Format = SCIFormat.Integer, .Name = "Status", .Value = Convert.ToInt32(nStatus)})


        Dim trans As SCITransaction = New SCITransaction With
        {
            .DeviceId = _equipment.DeviceID,
            .MessageType = MessageType.Primary,
            .Id = _transactionID + 1,
            .Name = "SendTrayStatus",
            .NeedReply = True,
            .Primary = primMsg,
            .Secondary = Nothing
        }

        SendMessage(trans)
    End Sub

#End Region


    '***************************************************************************
#Region "DispID16" '<DispId(16)> Sub RequestUnloadTray(ByVal strTrayBarcode As String, ByVal strLotNumber As String)
    Public Sub RequestUnloadTray(ByVal strTrayBarcode As String, ByVal strLotNumber As String) Implements IWDConnectWrapperLibClass.RequestUnloadTray
        Dim primMsg As SCIMessage = New SCIMessage
        primMsg.CommandID = "RequestUnloadTray"
        primMsg.Item = New SCIItem
        primMsg.Item.Format = SCIFormat.List
        primMsg.Item.Items = New SCIItemCollection

        primMsg.Item.Items.Add(New SCIItem With {.Format = SCIFormat.String, .Name = "MachineName", .Value = _equipment.MachineName})
        primMsg.Item.Items.Add(New SCIItem With {.Format = SCIFormat.String, .Name = "TrayBarcode", .Value = strTrayBarcode})
        primMsg.Item.Items.Add(New SCIItem With {.Format = SCIFormat.String, .Name = "LotNumber", .Value = strLotNumber})

        Dim trans As SCITransaction = New SCITransaction With
        {
            .DeviceId = _equipment.DeviceID,
            .MessageType = MessageType.Primary,
            .Id = _transactionID + 1,
            .Name = "RequestUnloadTray",
            .NeedReply = True,
            .Primary = primMsg,
            .Secondary = Nothing
        }

        SendMessage(trans)
    End Sub

#End Region


    '***************************************************************************
#Region "DispID17" '<DispId(17)> Sub RequestCancelProcessedLot(ByVal strLotNumber As String)
    Public Sub RequestCancelProcessedLot(ByVal strLotNumber As String) Implements IWDConnectWrapperLibClass.RequestCancelProcessedLot
        Dim primMsg As SCIMessage = New SCIMessage
        primMsg.CommandID = "RequestCancelProcessedLot"
        primMsg.Item = New SCIItem
        primMsg.Item.Format = SCIFormat.List
        primMsg.Item.Items = New SCIItemCollection

        primMsg.Item.Items.Add(New SCIItem With {.Format = SCIFormat.String, .Name = "MachineName", .Value = _equipment.MachineName})
        primMsg.Item.Items.Add(New SCIItem With {.Format = SCIFormat.String, .Name = "LotNumber", .Value = strLotNumber})

        Dim trans As SCITransaction = New SCITransaction With
        {
            .DeviceId = _equipment.DeviceID,
            .MessageType = MessageType.Primary,
            .Id = _transactionID + 1,
            .Name = "RequestCancelProcessedLot",
            .NeedReply = True,
            .Primary = primMsg,
            .Secondary = Nothing
        }

        SendMessage(trans)
    End Sub

#End Region


    '***************************************************************************
#Region "DispID18" '<DispId(18)> Sub RequestRemoveRemainTray(ByVal strLotNumber As String, ByVal strTrayBarcode1 As String, ByVal strTrayBarcode2 As String, ByVal strTrayBarcode3 As String, ByVal strTrayBarcode4 As String, ByVal strTrayBarcode5 As String, ByVal strTrayBarcode6 As String, ByVal strTrayBarcode7 As String, ByVal strTrayBarcode8 As String, ByVal strTrayBarcode9 As String, ByVal strTrayBarcode10 As String, ByVal strTrayBarcode11 As String, ByVal strTrayBarcode12 As String)
    Public Sub RequestRemoveRemainTray(ByVal strLotNumber As String,
                                       ByVal strTrayBarcode1 As String,
                                       ByVal strTrayBarcode2 As String,
                                       ByVal strTrayBarcode3 As String,
                                       ByVal strTrayBarcode4 As String,
                                       ByVal strTrayBarcode5 As String,
                                       ByVal strTrayBarcode6 As String,
                                       ByVal strTrayBarcode7 As String,
                                       ByVal strTrayBarcode8 As String,
                                       ByVal strTrayBarcode9 As String,
                                       ByVal strTrayBarcode10 As String,
                                       ByVal strTrayBarcode11 As String,
                                       ByVal strTrayBarcode12 As String) Implements IWDConnectWrapperLibClass.RequestRemoveRemainTray

        Dim primMsg As SCIMessage = New SCIMessage
        primMsg.CommandID = "RequestRemoveRemainTray"
        primMsg.Item = New SCIItem
        primMsg.Item.Format = SCIFormat.List
        primMsg.Item.Items = New SCIItemCollection

        primMsg.Item.Items.Add(New SCIItem With {.Format = SCIFormat.String, .Name = "MachineName", .Value = _equipment.MachineName})
        primMsg.Item.Items.Add(New SCIItem With {.Format = SCIFormat.String, .Name = "LotNumber", .Value = strLotNumber})

        primMsg.Item.Items.Add(New SCIItem With {.Format = SCIFormat.String, .Name = "RemoveTrayBarcode1", .Value = strTrayBarcode1})
        primMsg.Item.Items.Add(New SCIItem With {.Format = SCIFormat.String, .Name = "RemoveTrayBarcode2", .Value = strTrayBarcode2})
        primMsg.Item.Items.Add(New SCIItem With {.Format = SCIFormat.String, .Name = "RemoveTrayBarcode3", .Value = strTrayBarcode3})
        primMsg.Item.Items.Add(New SCIItem With {.Format = SCIFormat.String, .Name = "RemoveTrayBarcode4", .Value = strTrayBarcode4})
        primMsg.Item.Items.Add(New SCIItem With {.Format = SCIFormat.String, .Name = "RemoveTrayBarcode5", .Value = strTrayBarcode5})
        primMsg.Item.Items.Add(New SCIItem With {.Format = SCIFormat.String, .Name = "RemoveTrayBarcode6", .Value = strTrayBarcode6})
        primMsg.Item.Items.Add(New SCIItem With {.Format = SCIFormat.String, .Name = "RemoveTrayBarcode7", .Value = strTrayBarcode7})
        primMsg.Item.Items.Add(New SCIItem With {.Format = SCIFormat.String, .Name = "RemoveTrayBarcode8", .Value = strTrayBarcode8})
        primMsg.Item.Items.Add(New SCIItem With {.Format = SCIFormat.String, .Name = "RemoveTrayBarcode9", .Value = strTrayBarcode9})
        primMsg.Item.Items.Add(New SCIItem With {.Format = SCIFormat.String, .Name = "RemoveTrayBarcode10", .Value = strTrayBarcode10})
        primMsg.Item.Items.Add(New SCIItem With {.Format = SCIFormat.String, .Name = "RemoveTrayBarcode11", .Value = strTrayBarcode11})
        primMsg.Item.Items.Add(New SCIItem With {.Format = SCIFormat.String, .Name = "RemoveTrayBarcode12", .Value = strTrayBarcode12})

        Dim trans As SCITransaction = New SCITransaction With
        {
            .DeviceId = _equipment.DeviceID,
            .MessageType = MessageType.Primary,
            .Id = _transactionID + 1,
            .Name = "RequestRemoveRemainTray",
            .NeedReply = True,
            .Primary = primMsg,
            .Secondary = Nothing
        }

        SendMessage(trans)
    End Sub

#End Region


    '***************************************************************************
#Region "DispID19" '<DispId(19)> Sub ClosePack(ByVal strTrayBarcode1 As String, ByVal strTrayBarcode2 As String, ByVal strTrayBarcode3 As String, ByVal strTrayBarcode4 As String, ByVal strTrayBarcode5 As String, ByVal strTrayBarcode6 As String, ByVal strTrayBarcode7 As String, ByVal strTrayBarcode8 As String, ByVal strTrayBarcode9 As String, ByVal strTrayBarcode10 As String, ByVal strTrayBarcode11 As String, ByVal strTrayBarcode12 As String)
    Public Sub ClosePack(ByVal strTrayBarcode1 As String,
                         ByVal strTrayBarcode2 As String,
                         ByVal strTrayBarcode3 As String,
                         ByVal strTrayBarcode4 As String,
                         ByVal strTrayBarcode5 As String,
                         ByVal strTrayBarcode6 As String,
                         ByVal strTrayBarcode7 As String,
                         ByVal strTrayBarcode8 As String,
                         ByVal strTrayBarcode9 As String,
                         ByVal strTrayBarcode10 As String,
                         ByVal strTrayBarcode11 As String,
                         ByVal strTrayBarcode12 As String) Implements IWDConnectWrapperLibClass.ClosePack

        Dim primMsg As SCIMessage = New SCIMessage
        primMsg.CommandID = "ClosePack"
        primMsg.Item = New SCIItem
        primMsg.Item.Format = SCIFormat.List
        primMsg.Item.Items = New SCIItemCollection

        primMsg.Item.Items.Add(New SCIItem With {.Format = SCIFormat.String, .Name = "MachineName", .Value = _equipment.MachineName})

        primMsg.Item.Items.Add(New SCIItem With {.Format = SCIFormat.String, .Name = "TrayBarcode#1", .Value = strTrayBarcode1})
        primMsg.Item.Items.Add(New SCIItem With {.Format = SCIFormat.String, .Name = "TrayBarcode#2", .Value = strTrayBarcode2})
        primMsg.Item.Items.Add(New SCIItem With {.Format = SCIFormat.String, .Name = "TrayBarcode#3", .Value = strTrayBarcode3})
        primMsg.Item.Items.Add(New SCIItem With {.Format = SCIFormat.String, .Name = "TrayBarcode#4", .Value = strTrayBarcode4})
        primMsg.Item.Items.Add(New SCIItem With {.Format = SCIFormat.String, .Name = "TrayBarcode#5", .Value = strTrayBarcode5})
        primMsg.Item.Items.Add(New SCIItem With {.Format = SCIFormat.String, .Name = "TrayBarcode#6", .Value = strTrayBarcode6})
        primMsg.Item.Items.Add(New SCIItem With {.Format = SCIFormat.String, .Name = "TrayBarcode#7", .Value = strTrayBarcode7})
        primMsg.Item.Items.Add(New SCIItem With {.Format = SCIFormat.String, .Name = "TrayBarcode#8", .Value = strTrayBarcode8})
        primMsg.Item.Items.Add(New SCIItem With {.Format = SCIFormat.String, .Name = "TrayBarcode#9", .Value = strTrayBarcode9})
        primMsg.Item.Items.Add(New SCIItem With {.Format = SCIFormat.String, .Name = "TrayBarcode#10", .Value = strTrayBarcode10})
        primMsg.Item.Items.Add(New SCIItem With {.Format = SCIFormat.String, .Name = "TrayBarcode#11", .Value = strTrayBarcode11})
        primMsg.Item.Items.Add(New SCIItem With {.Format = SCIFormat.String, .Name = "TrayBarcode#12", .Value = strTrayBarcode12})

        Dim trans As SCITransaction = New SCITransaction With
        {
            .DeviceId = _equipment.DeviceID,
            .MessageType = MessageType.Primary,
            .Id = _transactionID + 1,
            .Name = "ClosePack",
            .NeedReply = True,
            .Primary = primMsg,
            .Secondary = Nothing
        }

        SendMessage(trans)
    End Sub

#End Region


    '***************************************************************************
#Region "DispID20" '<DispId(20)> Function GetCOMMACKFromXML(ByVal xmlGetCOMMACK As String) As Integer
    Public Function GetCOMMACKFromXML(ByVal xmlGetCOMMACK As String) As Integer Implements IWDConnectWrapperLibClass.GetCOMMACKFromXML
        Dim nCOMMACK As Integer = 0

        If (xmlGetCOMMACK.Length < 1) Then
            Return 1
        End If

        Dim getCOMMACKXMLDoc As XmlDocument = New XmlDocument()
        getCOMMACKXMLDoc.LoadXml(xmlGetCOMMACK)

        Dim root_getCOMMACKXMLDoc As XmlElement = getCOMMACKXMLDoc.DocumentElement
        Dim getCOMMACKNodeList As XmlNodeList = root_getCOMMACKXMLDoc.SelectNodes("/SCITransaction/Secondary/Item/Items/SCIItem")

        If (getCOMMACKNodeList.Count > 0) Then
            For Each node As XmlNode In getCOMMACKNodeList

                Dim nameNodeList As XmlNodeList = node.SelectNodes("./Name")
                Dim valueNodeList As XmlNodeList = node.SelectNodes("./Value")

                For Each nameChildNode As XmlNode In nameNodeList
                    If (nameChildNode.InnerText = "COMMACK") Then

                        For Each valueChildNode As XmlNode In valueNodeList
                            nCOMMACK = Convert.ToInt32(valueChildNode.InnerText, 10)
                        Next

                    End If
                Next



            Next node
        End If

        Return nCOMMACK
    End Function

#End Region


    '***************************************************************************
#Region "DispID21" '<DispId(21)> Function GetRequestUnloadTrayResultFromXML(ByVal xmlGetRequestUnloadTrayResult As String) As Boolean
    Public Function GetRequestUnloadTrayResultFromXML(ByVal xmlGetRequestUnloadTrayResult As String) As Boolean Implements IWDConnectWrapperLibClass.GetRequestUnloadTrayResultFromXML
        Dim bCanUnload As Boolean = False

        If (xmlGetRequestUnloadTrayResult.Length < 1) Then
            Return False
        End If

        Dim getRequestUnloadTrayResultXMLDoc As XmlDocument = New XmlDocument()
        getRequestUnloadTrayResultXMLDoc.LoadXml(xmlGetRequestUnloadTrayResult)

        Dim root_getRequestUnloadTrayResultXMLDoc As XmlElement = getRequestUnloadTrayResultXMLDoc.DocumentElement
        Dim getCOMMACKNodeList As XmlNodeList = root_getRequestUnloadTrayResultXMLDoc.SelectNodes("/SCITransaction/Secondary/Item/Items/SCIItem")

        If (getCOMMACKNodeList.Count > 0) Then
            For Each node As XmlNode In getCOMMACKNodeList

                Dim nameNodeList As XmlNodeList = node.SelectNodes("./Name")
                Dim valueNodeList As XmlNodeList = node.SelectNodes("./Value")

                For Each nameChildNode As XmlNode In nameNodeList
                    If (nameChildNode.InnerText = "CanUnload") Then

                        For Each valueChildNode As XmlNode In valueNodeList
                            bCanUnload = Convert.ToBoolean(valueChildNode.InnerText)
                        Next

                    End If
                Next

            Next node
        End If

        Return bCanUnload
    End Function

#End Region


    '***************************************************************************
#Region "DispID22" '<DispId(22)> Property ControlState() As CONTROL_STATE
    Public Property ControlState() As CONTROL_STATE Implements IWDConnectWrapperLibClass.ControlState
        Get
            Return _equipment.ControlState
        End Get
        Set(ByVal value As CONTROL_STATE)
            _equipment.ControlState = value
        End Set
    End Property

#End Region


    '***************************************************************************
#Region "DispID23" '<DispId(23)> Sub SendControlState()
    Public Sub SendControlState() Implements IWDConnectWrapperLibClass.SendControlState
        Log.Info("WDConnectWrapperLibClass.SendControlState")
        SendControlState(_equipment.ControlState)
    End Sub

#End Region


    '***************************************************************************
    Public Sub SendControlState(ByVal controlState As CONTROL_STATE)
        Dim primMsg As SCIMessage = New SCIMessage
        primMsg.CommandID = "SendControlState"
        primMsg.Item = New SCIItem
        primMsg.Item.Format = SCIFormat.List
        primMsg.Item.Items = New SCIItemCollection

        primMsg.Item.Items.Add(New SCIItem With {.Format = SCIFormat.Integer, .Name = "ControlState", .Value = Convert.ToInt32(controlState)})
        primMsg.Item.Items.Add(New SCIItem With {.Format = SCIFormat.String, .Name = "MachineName", .Value = _equipment.MachineName})


        Dim trans As SCITransaction = New SCITransaction With
        {
            .DeviceId = _equipment.DeviceID,
            .MessageType = MessageType.Primary,
            .Id = _transactionID + 1,
            .Name = "SendControlState",
            .NeedReply = True,
            .Primary = primMsg,
            .Secondary = Nothing
        }

        SendMessage(trans)

    End Sub


    '***************************************************************************
#Region "DispID24" '<DispId(24)> Function GetALIDFromXML(ByVal xmlGetALID As String) As Integer
    Public Function GetALIDFromXML(ByVal xmlGetALID As String) As Integer Implements IWDConnectWrapperLibClass.GetALIDFromXML
        Dim nALID As Integer = 0

        If (xmlGetALID.Length < 1) Then
            Return 1
        End If

        Dim getALIDXMLDoc As XmlDocument = New XmlDocument()
        getALIDXMLDoc.LoadXml(xmlGetALID)

        Dim root_getALIDXMLDoc As XmlElement = getALIDXMLDoc.DocumentElement
        Dim getALIDNodeList As XmlNodeList = root_getALIDXMLDoc.SelectNodes("/SCITransaction/Secondary/Item/Items/SCIItem")

        If (getALIDNodeList.Count > 0) Then
            For Each node As XmlNode In getALIDNodeList

                Dim nameNodeList As XmlNodeList = node.SelectNodes("./Name")
                Dim valueNodeList As XmlNodeList = node.SelectNodes("./Value")

                For Each nameChildNode As XmlNode In nameNodeList
                    If (nameChildNode.InnerText = "ALID") Then

                        For Each valueChildNode As XmlNode In valueNodeList
                            nALID = Convert.ToInt32(valueChildNode.InnerText, 10)
                        Next

                    End If
                Next



            Next node
        End If

        Return nALID
    End Function

#End Region


    '***************************************************************************
#Region "DispID25" '<DispId(25)> Sub SendAlarmReport(ByVal alarm As Alarm)
    Public Sub SendAlarmReport(ByVal alarm As Alarm) Implements IWDConnectWrapperLibClass.SendAlarmReport
        Dim primMsg As SCIMessage = New SCIMessage
        primMsg.CommandID = "AlarmReportSend"
        primMsg.Item = New SCIItem
        primMsg.Item.Format = SCIFormat.List
        primMsg.Item.Items = New SCIItemCollection

        primMsg.Item.Items.Add(New SCIItem With {.Format = SCIFormat.Integer, .Name = "ALID", .Value = alarm.AlID})
        primMsg.Item.Items.Add(New SCIItem With {.Format = SCIFormat.String, .Name = "AlarmText", .Value = alarm.AlText})


        Dim trans As SCITransaction = New SCITransaction With
        {
            .DeviceId = _equipment.DeviceID,
            .MessageType = MessageType.Primary,
            .Id = _transactionID + 1,
            .Name = "AlarmReportSend",
            .NeedReply = True,
            .Primary = primMsg,
            .Secondary = Nothing
        }

        SendMessage(trans)
    End Sub

#End Region


    '***************************************************************************
#Region "DispID26" '<DispId(26)> Sub SendPkgSetting(ByVal pkgObj As PkgSetting)
    Public Sub SendPkgSetting(ByVal pkgObj As PkgSetting) Implements IWDConnectWrapperLibClass.SendPkgSetting
        Dim primMsg As SCIMessage = New SCIMessage
        primMsg.CommandID = "SendPkgSetting"
        primMsg.Item = New SCIItem
        primMsg.Item.Format = SCIFormat.List
        primMsg.Item.Items = New SCIItemCollection

        primMsg.Item.Items.Add(New SCIItem With {.Format = SCIFormat.Float, .Name = "PitchOffset", .Value = pkgObj.PitchOffset})
        primMsg.Item.Items.Add(New SCIItem With {.Format = SCIFormat.Float, .Name = "RollOffset", .Value = pkgObj.RollOffset})
        primMsg.Item.Items.Add(New SCIItem With {.Format = SCIFormat.Float, .Name = "RollOffset", .Value = pkgObj.ZHeightOffset})

        primMsg.Item.Items.Add(New SCIItem With {.Format = SCIFormat.Float, .Name = "RollOffset", .Value = pkgObj.PitchUSL})
        primMsg.Item.Items.Add(New SCIItem With {.Format = SCIFormat.Float, .Name = "RollOffset", .Value = pkgObj.PitchLSL})
        primMsg.Item.Items.Add(New SCIItem With {.Format = SCIFormat.Float, .Name = "RollOffset", .Value = pkgObj.RollUSL})
        primMsg.Item.Items.Add(New SCIItem With {.Format = SCIFormat.Float, .Name = "RollOffset", .Value = pkgObj.RollLSL})

        primMsg.Item.Items.Add(New SCIItem With {.Format = SCIFormat.Float, .Name = "RollOffset", .Value = pkgObj.ZhtTarget})

        primMsg.Item.Items.Add(New SCIItem With {.Format = SCIFormat.Float, .Name = "RollOffset", .Value = pkgObj.PitchSensitivity})
        primMsg.Item.Items.Add(New SCIItem With {.Format = SCIFormat.Float, .Name = "RollOffset", .Value = pkgObj.PRAdjAcceptTol})

        primMsg.Item.Items.Add(New SCIItem With {.Format = SCIFormat.Float, .Name = "RollOffset", .Value = pkgObj.PitchAdjCL})
        primMsg.Item.Items.Add(New SCIItem With {.Format = SCIFormat.Float, .Name = "RollOffset", .Value = pkgObj.PitchAdjUCL})
        primMsg.Item.Items.Add(New SCIItem With {.Format = SCIFormat.Float, .Name = "RollOffset", .Value = pkgObj.PitchAdjLCL})

        primMsg.Item.Items.Add(New SCIItem With {.Format = SCIFormat.Float, .Name = "RollOffset", .Value = pkgObj.RollAdjCL})
        primMsg.Item.Items.Add(New SCIItem With {.Format = SCIFormat.Float, .Name = "RollOffset", .Value = pkgObj.RollAdjUCL})
        primMsg.Item.Items.Add(New SCIItem With {.Format = SCIFormat.Float, .Name = "RollOffset", .Value = pkgObj.RollAdjLCL})


        Dim trans As SCITransaction = New SCITransaction With
        {
            .DeviceId = _equipment.DeviceID,
            .MessageType = MessageType.Primary,
            .Id = _transactionID + 1,
            .Name = "SendPkgSetting",
            .NeedReply = True,
            .Primary = primMsg,
            .Secondary = Nothing
        }

        SendMessage(trans)
    End Sub


#End Region


    '***************************************************************************
#Region "DispID27"
    '<DispId(27)> ReadOnly Property T3Timeout() As Integer
    Public ReadOnly Property T3Timeout() As Integer Implements IWDConnectWrapperLibClass.T3Timeout
        Get
            Return _equipment.T3Timeout
        End Get
    End Property
#End Region


    '***************************************************************************
#Region "DispID28"
    '<DispId(28)> ReadOnly Property T5Timeout() As Integer
    Public ReadOnly Property T5Timeout() As Integer Implements IWDConnectWrapperLibClass.T5Timeout
        Get
            Return _equipment.T5Timeout
        End Get
    End Property
#End Region


    '***************************************************************************
#Region "DispID29"
    '<DispId(29)> ReadOnly Property T6Timeout() As Integer
    Public ReadOnly Property T6Timeout() As Integer Implements IWDConnectWrapperLibClass.T6Timeout
        Get
            Return _equipment.T6Timeout
        End Get
    End Property
#End Region


    '***************************************************************************
#Region "DispID30"
    '<DispId(30)> ReadOnly Property T7Timeout() As Integer
    Public ReadOnly Property T7Timeout() As Integer Implements IWDConnectWrapperLibClass.T7Timeout
        Get
            Return _equipment.T7Timeout
        End Get
    End Property
#End Region


    '***************************************************************************
#Region "DispID31"
    '<DispId(31)> ReadOnly Property T8Timeout() As Integer
    Public ReadOnly Property T8Timeout() As Integer Implements IWDConnectWrapperLibClass.T8Timeout
        Get
            Return _equipment.T8Timeout
        End Get
    End Property
#End Region


    '***************************************************************************
#Region "DispID32"
    '<DispId(32)> ReadOnly Property MachineID As String
    Public ReadOnly Property MachineName As String Implements IWDConnectWrapperLibClass.MachineName
        Get
            Return _equipment.MachineName
        End Get
    End Property
#End Region




    '***************************************************************************


    '***************************************************************************
    Public Sub SendAlarmReportAck(ByVal priTrans As SCITransaction)
        Dim scndMsg As SCIMessage = New SCIMessage
        scndMsg.CommandID = "AlarmReportSendAck"
        scndMsg.Item = New SCIItem
        scndMsg.Item.Format = SCIFormat.List
        scndMsg.Item.Items = New SCIItemCollection

        scndMsg.Item.Items.Add(New SCIItem With {.Format = SCIFormat.Integer, .Name = "COMMACK", .Value = 0})

        Dim trans As SCITransaction = New SCITransaction With
        {
            .DeviceId = _equipment.DeviceID,
            .MessageType = MessageType.Secondary,
            .Id = priTrans.Id,
            .Name = "AlarmReportSendAck",
            .NeedReply = False,
            .Primary = priTrans.Primary,
            .Secondary = scndMsg
        }

        SendMessage(trans)
    End Sub



    Public Sub RejectCommandWhileOffline()
        Dim scndMsg As SCIMessage = New SCIMessage
        scndMsg.CommandID = "RejectCommandWhileOffline"
        scndMsg.Item = New SCIItem
        scndMsg.Item.Format = SCIFormat.List
        scndMsg.Item.Items = New SCIItemCollection

        scndMsg.Item.Items.Add(New SCIItem With {.Format = SCIFormat.Integer, .Name = "COMMACK", .Value = 1})

        Dim trans As SCITransaction = New SCITransaction With
        {
            .DeviceId = _equipment.DeviceID,
            .MessageType = MessageType.Secondary,
            .Id = _transactionID,
            .Name = "RejectCommandWhileOffline",
            .NeedReply = False,
            .Primary = Nothing,
            .Secondary = scndMsg
        }

        SendMessage(trans)
    End Sub



    Public Sub RejectCommandWhileOffline(ByVal priTrans As SCITransaction)
        Dim scndMsg As SCIMessage = New SCIMessage
        scndMsg.CommandID = "RejectCommandWhileOffline"
        scndMsg.Item = New SCIItem
        scndMsg.Item.Format = SCIFormat.List
        scndMsg.Item.Items = New SCIItemCollection

        scndMsg.Item.Items.Add(New SCIItem With {.Format = SCIFormat.Integer, .Name = "COMMACK", .Value = 1})

        Dim trans As SCITransaction = New SCITransaction With
        {
            .DeviceId = _equipment.DeviceID,
            .MessageType = MessageType.Secondary,
            .Id = priTrans.Id,
            .Name = "RejectCommandWhileOffline",
            .NeedReply = False,
            .Primary = priTrans.Primary,
            .Secondary = scndMsg
        }

        SendMessage(trans)
    End Sub



    Public Sub OnlineRequestAck()
        Dim scndMsg As SCIMessage = New SCIMessage
        scndMsg.CommandID = "OnlineRequestAck"
        scndMsg.Item = New SCIItem
        scndMsg.Item.Format = SCIFormat.List
        scndMsg.Item.Items = New SCIItemCollection

        scndMsg.Item.Items.Add(New SCIItem With {.Format = SCIFormat.Integer, .Name = "COMMACK", .Value = 0})

        Dim trans As SCITransaction = New SCITransaction With
        {
            .DeviceId = _equipment.DeviceID,
            .MessageType = MessageType.Secondary,
            .Id = _transactionID,
            .Name = "OnlineRequestAck",
            .NeedReply = False,
            .Primary = Nothing,
            .Secondary = scndMsg
        }

        SendMessage(trans)
    End Sub



    Public Sub OnlineRequestAck(ByVal priTrans As SCITransaction)
        Dim scndMsg As SCIMessage = New SCIMessage
        scndMsg.CommandID = "OnlineRequestAck"
        scndMsg.Item = New SCIItem
        scndMsg.Item.Format = SCIFormat.List
        scndMsg.Item.Items = New SCIItemCollection

        scndMsg.Item.Items.Add(New SCIItem With {.Format = SCIFormat.Integer, .Name = "COMMACK", .Value = 0})

        Dim trans As SCITransaction = New SCITransaction With
        {
            .DeviceId = _equipment.DeviceID,
            .MessageType = MessageType.Secondary,
            .Id = priTrans.Id,
            .Name = "OnlineRequestAck",
            .NeedReply = False,
            .Primary = priTrans.Primary,
            .Secondary = scndMsg
        }

        SendMessage(trans)
    End Sub



    Public Sub OfflineRequestAck()
        Dim scndMsg As SCIMessage = New SCIMessage
        scndMsg.CommandID = "OfflineRequestAck"
        scndMsg.Item = New SCIItem
        scndMsg.Item.Format = SCIFormat.List
        scndMsg.Item.Items = New SCIItemCollection

        scndMsg.Item.Items.Add(New SCIItem With {.Format = SCIFormat.Integer, .Name = "COMMACK", .Value = 0})

        Dim trans As SCITransaction = New SCITransaction With
        {
            .DeviceId = _equipment.DeviceID,
            .MessageType = MessageType.Secondary,
            .Id = _transactionID,
            .Name = "OfflineRequestAck",
            .NeedReply = False,
            .Primary = Nothing,
            .Secondary = scndMsg
        }

        SendMessage(trans)
    End Sub



    Public Sub OfflineRequestAck(ByVal priTrans As SCITransaction)
        Dim scndMsg As SCIMessage = New SCIMessage
        scndMsg.CommandID = "OfflineRequestAck"
        scndMsg.Item = New SCIItem
        scndMsg.Item.Format = SCIFormat.List
        scndMsg.Item.Items = New SCIItemCollection

        scndMsg.Item.Items.Add(New SCIItem With {.Format = SCIFormat.Integer, .Name = "COMMACK", .Value = 0})

        Dim trans As SCITransaction = New SCITransaction With
        {
            .DeviceId = _equipment.DeviceID,
            .MessageType = MessageType.Secondary,
            .Id = priTrans.Id,
            .Name = "OfflineRequestAck",
            .NeedReply = False,
            .Primary = priTrans.Primary,
            .Secondary = scndMsg
        }

        SendMessage(trans)
    End Sub



    Public Sub RequestProcessStateAck()
        Dim scndMsg As SCIMessage = New SCIMessage
        scndMsg.CommandID = "RequestProcessStateAck"
        scndMsg.Item = New SCIItem
        scndMsg.Item.Format = SCIFormat.List
        scndMsg.Item.Items = New SCIItemCollection

        scndMsg.Item.Items.Add(New SCIItem With {.Format = SCIFormat.Integer, .Name = "COMMACK", .Value = 0})
        scndMsg.Item.Items.Add(New SCIItem With {.Format = SCIFormat.Integer, .Name = "ProcessState", .Value = Convert.ToInt32(ProcessState)})
        scndMsg.Item.Items.Add(New SCIItem With {.Format = SCIFormat.String, .Name = "MachineName", .Value = _equipment.MachineName})

        Dim trans As SCITransaction = New SCITransaction With
        {
            .DeviceId = _equipment.DeviceID,
            .MessageType = MessageType.Secondary,
            .Id = _transactionID,
            .Name = "RequestProcessStateAck",
            .NeedReply = False,
            .Primary = Nothing,
            .Secondary = scndMsg
        }

        SendMessage(trans)
    End Sub



    Public Sub RequestProcessStateAck(ByVal priTrans As SCITransaction)
        Dim scndMsg As SCIMessage = New SCIMessage
        scndMsg.CommandID = "RequestProcessStateAck"
        scndMsg.Item = New SCIItem
        scndMsg.Item.Format = SCIFormat.List
        scndMsg.Item.Items = New SCIItemCollection

        scndMsg.Item.Items.Add(New SCIItem With {.Format = SCIFormat.Integer, .Name = "COMMACK", .Value = 0})
        scndMsg.Item.Items.Add(New SCIItem With {.Format = SCIFormat.Integer, .Name = "ProcessState", .Value = Convert.ToInt32(ProcessState)})
        scndMsg.Item.Items.Add(New SCIItem With {.Format = SCIFormat.String, .Name = "MachineName", .Value = _equipment.MachineName})

        Dim trans As SCITransaction = New SCITransaction With
        {
            .DeviceId = _equipment.DeviceID,
            .MessageType = MessageType.Secondary,
            .Id = priTrans.Id,
            .Name = "RequestProcessStateAck",
            .NeedReply = False,
            .Primary = priTrans.Primary,
            .Secondary = scndMsg
        }

        SendMessage(trans)
    End Sub



    Public Sub RequestControlStateAck()
        Dim scndMsg As SCIMessage = New SCIMessage
        scndMsg.CommandID = "RequestControlStateAck"
        scndMsg.Item = New SCIItem
        scndMsg.Item.Format = SCIFormat.List
        scndMsg.Item.Items = New SCIItemCollection

        scndMsg.Item.Items.Add(New SCIItem With {.Format = SCIFormat.Integer, .Name = "COMMACK", .Value = 0})
        scndMsg.Item.Items.Add(New SCIItem With {.Format = SCIFormat.Integer, .Name = "ControlState", .Value = Convert.ToInt32(ControlState)})
        scndMsg.Item.Items.Add(New SCIItem With {.Format = SCIFormat.String, .Name = "MachineName", .Value = _equipment.MachineName})

        Dim trans As SCITransaction = New SCITransaction With
        {
            .DeviceId = _equipment.DeviceID,
            .MessageType = MessageType.Secondary,
            .Id = _transactionID,
            .Name = "RequestControlStateAck",
            .NeedReply = False,
            .Primary = Nothing,
            .Secondary = scndMsg
        }

        SendMessage(trans)
    End Sub



    Public Sub RequestControlStateAck(ByVal priTrans As SCITransaction)
        Dim scndMsg As SCIMessage = New SCIMessage
        scndMsg.CommandID = "RequestControlStateAck"
        scndMsg.Item = New SCIItem
        scndMsg.Item.Format = SCIFormat.List
        scndMsg.Item.Items = New SCIItemCollection

        scndMsg.Item.Items.Add(New SCIItem With {.Format = SCIFormat.Integer, .Name = "COMMACK", .Value = 0})
        scndMsg.Item.Items.Add(New SCIItem With {.Format = SCIFormat.Integer, .Name = "ControlState", .Value = Convert.ToInt32(ControlState)})
        scndMsg.Item.Items.Add(New SCIItem With {.Format = SCIFormat.String, .Name = "MachineName", .Value = _equipment.MachineName})

        Dim trans As SCITransaction = New SCITransaction With
        {
            .DeviceId = _equipment.DeviceID,
            .MessageType = MessageType.Secondary,
            .Id = priTrans.Id,
            .Name = "RequestControlStateAck",
            .NeedReply = False,
            .Primary = priTrans.Primary,
            .Secondary = scndMsg
        }

        SendMessage(trans)
    End Sub



    Public Sub SendProcessState(ByVal processState As PROCESS_STATE)
        Dim primMsg As SCIMessage = New SCIMessage
        primMsg.CommandID = "SendProcessState"
        primMsg.Item = New SCIItem
        primMsg.Item.Format = SCIFormat.List
        primMsg.Item.Items = New SCIItemCollection

        primMsg.Item.Items.Add(New SCIItem With {.Format = SCIFormat.Integer, .Name = "ProcessState", .Value = Convert.ToInt32(processState)})
        primMsg.Item.Items.Add(New SCIItem With {.Format = SCIFormat.String, .Name = "MachineName", .Value = _equipment.MachineName})


        Dim trans As SCITransaction = New SCITransaction With
        {
            .DeviceId = _equipment.DeviceID,
            .MessageType = MessageType.Primary,
            .Id = _transactionID + 1,
            .Name = "SendProcessState",
            .NeedReply = True,
            .Primary = primMsg,
            .Secondary = Nothing
        }

        SendMessage(trans)

    End Sub



End Class



'***************************************************************************
#Region "Enum"
'***************************************************************************
#Region "Enum PROCESS_STATE"
Public Enum PROCESS_STATE As Integer
    _INIT = 0
    _IDLE = 1
    _HOME = 2
    _EMERGENCY = 3
    _READY = 4
    _LOOP = 5
    _OPERATOR = 6
    _STEP = 7
    _SETUP = 8
    _ABORT = 9
End Enum
#End Region



'***************************************************************************
#Region "Enum TRAY_STATUS"
Public Enum TRAY_STATUS As Integer
    NA = 0
    LOADING = 1
    LOADED = 2
    PROCESSING = 3
    PROCESSED = 4
    HOLDING = 5
    UNLOADING = 6
    UNLOADED = 7
End Enum
#End Region



'***************************************************************************
#Region "Enum LOT_STATUS"
Public Enum LOT_STATUS As Integer
    CAN_ADJUST = 0
    CANNOT_ADJUST = 1
End Enum
#End Region



'***************************************************************************
#Region "Enum CONTROL_STATE"
Public Enum CONTROL_STATE As Integer
    NA = 0
    OFFLINE = 1
    OFFLINE_ATTEMPT_ONLINE = 2
    OFFLINE_HOST_OFFLINE = 3
    ONLINE_LOCAL = 4
    ONLINE_REMOTE = 5

End Enum
#End Region


#End Region '"Enum"


'***************************************************************************
#Region "Class EquipmentController"
Public Class EquipmentController : Inherits WDConnect.Application.WDConnectBase

    Public ReadOnly Log As log4net.ILog
    Public _processState As PROCESS_STATE
    Public _controlState As CONTROL_STATE

    Public Sub New()
        Log = log4net.LogManager.GetLogger("MainWindow")

        Dim configPath As String = System.Windows.Forms.Application.StartupPath
        Dim configFile As System.IO.FileInfo
        configFile = New System.IO.FileInfo(configPath + "\WDLog.config")

        log4net.Config.XmlConfigurator.Configure(configFile)

        _processState = New PROCESS_STATE()
        _processState = PROCESS_STATE._INIT

        _controlState = New CONTROL_STATE()
        _controlState = CONTROL_STATE.OFFLINE
    End Sub

    Public Overrides Sub Initialize(ByVal equipmentModelPath As String)
        MyBase.Initialize(equipmentModelPath)
    End Sub

    Public Sub SendMessage(ByVal transaction As WDConnect.Common.SCITransaction)
        Log.Info("EquipmentController SendMessage")

        Try
            MyBase.ProcessOutStream(transaction)
        Catch ex As Exception
            Log.Info("SendMessage, Exception: " + ex.InnerException.ToString())
        End Try
    End Sub

    Public Sub ReplyOutStream(ByVal transaction As WDConnect.Common.SCITransaction)
        MyBase.ReplyOutSteam(transaction)
    End Sub

    Public Function ToolID() As String
        Return EquipmentModel.Nameable.id
    End Function

    Public Function DeviceID() As String
        Return EquipmentModel.GemConnection.deviceId
    End Function

    Public Function MachineName() As String
        Return EquipmentModel.Nameable.alias
    End Function

    Public Function SoftwareRevision() As String
        Return EquipmentModel.Nameable.softwareRev
    End Function

    Public Function ConnectionMode() As String
        Return EquipmentModel.GemConnection.HSMS.connectionMode.ToString()
    End Function

    Public Function RemoteIPAddress() As String
        Return EquipmentModel.GemConnection.HSMS.remoteIPAddress
    End Function

    Public Function RemotePortNumber() As String
        Return EquipmentModel.GemConnection.HSMS.remotePortNumber.ToString()
    End Function

    Public Function T3Timeout() As Integer
        Return EquipmentModel.GemConnection.HSMS.T3Timeout
    End Function

    Public Function T5Timeout() As Integer
        Return EquipmentModel.GemConnection.HSMS.T5Timeout
    End Function

    Public Function T6Timeout() As Integer
        Return EquipmentModel.GemConnection.HSMS.T6Timeout
    End Function

    Public Function T7Timeout() As Integer
        Return EquipmentModel.GemConnection.HSMS.T7Timeout
    End Function

    Public Function T8Timeout() As Integer
        Return EquipmentModel.GemConnection.HSMS.T8Timeout
    End Function

    Public Property ProcessState() As PROCESS_STATE
        Get
            Return _processState
        End Get
        Set(ByVal value As PROCESS_STATE)
            _processState = value
        End Set
    End Property


    Public Property ControlState() As CONTROL_STATE
        Get
            Return _controlState
        End Get
        Set(ByVal value As CONTROL_STATE)
            _controlState = value
        End Set
    End Property

End Class
#End Region



'***************************************************************************
Public Class BuilderSetting

    Private _rootPath As String = AppConfig.GetString("ModelBuilder", "RootPath", ".\Local\BuilderSetting")
    Public Property RoothPath As String
        Get
            Return _rootPath
        End Get
        Set(ByVal value As String)
            _rootPath = value
        End Set
    End Property


    Private _modelsPath As String = AppConfig.GetString("ModelBuilder", "ToolModelsPath", ".\Local\ToolModels")
    Public Property ModelsPath As String
        Get
            Return _modelsPath
        End Get
        Set(ByVal value As String)
            _modelsPath = value
        End Set
    End Property


    Private _settingPath As String = AppConfig.GetString("ModelBuilder", "BuilderSettingFilePath", ".\Local\ToolModels")
    Public Property SettingPath As String
        Get
            Return _settingPath
        End Get
        Set(ByVal value As String)
            _settingPath = value
        End Set
    End Property


    Private _defaultToolModelFileExtension As String = AppConfig.GetString("ModelBuilder", "DefaultToolModelFileExtension", "xml")
    Public Property DefaultToolModelFileExtension As String
        Get
            Return _defaultToolModelFileExtension
        End Get
        Set(ByVal value As String)
            _defaultToolModelFileExtension = value
        End Set
    End Property


    Private _messageFileNameFormat As String = AppConfig.GetString("ModelBuilder", "MessageFileNameFormat", "[ToolModel]_[MessageID]")
    Public Property MessageFileNameFormat
        Get
            Return _messageFileNameFormat
        End Get
        Set(ByVal value)
            _messageFileNameFormat = value
        End Set
    End Property


    Private _defaultMessageFileExtension As String = AppConfig.GetString("ModelBuilder", "DefaultMessageFileExtension", "msg")
    Public Property DefaultMessageFileExtension
        Get
            Return _defaultMessageFileExtension
        End Get
        Set(ByVal value)
            _defaultMessageFileExtension = value
        End Set
    End Property


    Private _messageFilePath As String = AppConfig.GetString("ModelBuilder", "MessageFilePath", ".\Local\Messages")
    Public Property MessageFilePath As String
        Get
            Return _messageFilePath
        End Get
        Set(ByVal value As String)
            _messageFilePath = value
        End Set
    End Property


    Private Shared _instance As BuilderSetting
    Public Shared Function Instance() As BuilderSetting
        If _instance Is Nothing Then
            _instance = New BuilderSetting
        End If

        Return _instance
    End Function

End Class



'***************************************************************************
<Guid("BC92556B-B093-4B97-8EC6-15D2E55F9206"), InterfaceType(ComInterfaceType.InterfaceIsIDispatch)>
Public Interface IProcessTrayClass
    <DispId(1)> Property TrayBarcode() As String
    <DispId(2)> Property LotNumber() As String
    <DispId(3)> Function GetHga(ByVal index As Integer) As ProcessHGA
    <DispId(4)> Sub SetHga(ByVal index As Integer, ByVal hga As ProcessHGA)
    <DispId(5)> Property HGAPartNumber() As String
    <DispId(6)> Property ProgName() As String
    <DispId(7)> Property HeadType() As String
    <DispId(8)> Property AdjustFixtureID() As String
    <DispId(9)> Property NetSpecCode() As String
    <DispId(10)> Property ReadType() As String
    <DispId(11)> Property SuspLot() As String
    <DispId(12)> Property STR() As String
    <DispId(13)> Property CoverTrayBarcode() As String
    <DispId(14)> Property CoverTrayMitecs() As String
    <DispId(15)> Property LaserADJPowerTopBot() As String
    <DispId(16)> Property LineAssyMC() As String
    <DispId(17)> Property OperatorID() As String
End Interface


<Guid("5E5AB606-7F9A-408F-A9EA-29F05388D065"), ClassInterface(ClassInterfaceType.None), ProgId("WDConnectWrapperLib.ProcessTray")>
Public Class ProcessTray
    Implements IProcessTrayClass

    'ctor
    Public Sub New()

        For i As Integer = 0 To 59
            _hga(i) = New ProcessHGA()
        Next i

    End Sub

    'dtor
    Protected Overrides Sub Finalize()

    End Sub


    Private _traybarcode As String = ""
    Public Property TrayBarcode As String Implements IProcessTrayClass.TrayBarcode
        Get
            Return _traybarcode
        End Get
        Set(ByVal value As String)
            _traybarcode = value
        End Set
    End Property


    Private _lotnumber As String = ""
    Public Property LotNumber As String Implements IProcessTrayClass.LotNumber
        Get
            Return _lotnumber
        End Get
        Set(ByVal value As String)
            _lotnumber = value
        End Set
    End Property


    Private _hgaPartNumber As String = ""
    Public Property HGAPartNumber As String Implements IProcessTrayClass.HGAPartNumber
        Get
            Return _hgaPartNumber
        End Get
        Set(ByVal value As String)
            _hgaPartNumber = value
        End Set
    End Property



    Private _progName As String = ""
    Public Property ProgName As String Implements IProcessTrayClass.ProgName
        Get
            Return _progName
        End Get
        Set(ByVal value As String)
            _progName = value
        End Set
    End Property



    Private _headType As String = ""
    Public Property HeadType As String Implements IProcessTrayClass.HeadType
        Get
            Return _headType
        End Get
        Set(ByVal value As String)
            _headType = value
        End Set
    End Property


    Private _adjustFixtureID As String = ""
    Public Property AdjustFixtureID As String Implements IProcessTrayClass.AdjustFixtureID
        Get
            Return _adjustFixtureID
        End Get
        Set(ByVal value As String)
            _adjustFixtureID = value
        End Set
    End Property



    Private _netSpecCode As String = ""
    Public Property NetSpecCode As String Implements IProcessTrayClass.NetSpecCode
        Get
            Return _netSpecCode
        End Get
        Set(ByVal value As String)
            _netSpecCode = value
        End Set
    End Property



    Private _readType As String = ""
    Public Property ReadType As String Implements IProcessTrayClass.ReadType
        Get
            Return _readType
        End Get
        Set(ByVal value As String)
            _readType = value
        End Set
    End Property



    Private _suspLot As String = ""
    Public Property SuspLot As String Implements IProcessTrayClass.SuspLot
        Get
            Return _suspLot
        End Get
        Set(ByVal value As String)
            _suspLot = value
        End Set
    End Property



    Private _str As String = ""
    Public Property STR As String Implements IProcessTrayClass.STR
        Get
            Return _str
        End Get
        Set(ByVal value As String)
            _str = value
        End Set
    End Property



    Private _coverTrayBarcode As String = ""
    Public Property CoverTrayBarcode As String Implements IProcessTrayClass.CoverTrayBarcode
        Get
            Return _coverTrayBarcode
        End Get
        Set(ByVal value As String)
            _coverTrayBarcode = value
        End Set
    End Property



    Private _coverTrayMitecs As String = ""
    Public Property CoverTrayMitecs As String Implements IProcessTrayClass.CoverTrayMitecs
        Get
            Return _coverTrayMitecs
        End Get
        Set(ByVal value As String)
            _coverTrayMitecs = value
        End Set
    End Property



    Private _laserADJPowerTopBot As String = ""
    Public Property LaserADJPowerTopBot As String Implements IProcessTrayClass.LaserADJPowerTopBot
        Get
            Return _laserADJPowerTopBot
        End Get
        Set(ByVal value As String)
            _laserADJPowerTopBot = value
        End Set
    End Property



    Private _lineAssyMC As String = ""
    Public Property LineAssyMC As String Implements IProcessTrayClass.LineAssyMC
        Get
            Return _lineAssyMC
        End Get
        Set(ByVal value As String)
            _lineAssyMC = value
        End Set
    End Property



    Private _operatorID As String = ""
    Public Property OperatorID As String Implements IProcessTrayClass.OperatorID
        Get
            Return _operatorID
        End Get
        Set(ByVal value As String)
            _operatorID = value
        End Set
    End Property




    Public _hga(59) As ProcessHGA
    Public Function GetHga(ByVal index As Integer) As ProcessHGA Implements IProcessTrayClass.GetHga
        Return _hga(index)
    End Function


    Public Sub SetHga(ByVal index As Integer, ByVal hga As ProcessHGA) Implements IProcessTrayClass.SetHga
        _hga(index) = hga
    End Sub


End Class



'***************************************************************************
<Guid("D13F7C80-5FE8-458E-A221-7B52D16CCD1F"), InterfaceType(ComInterfaceType.InterfaceIsIDispatch)>
Public Interface ITrayInfoClass
    <DispId(1)> Property TrayBarcode() As String
    <DispId(2)> Property IsInProcessLot() As Boolean
    <DispId(3)> Property LotNumber() As String

    <DispId(4)> Property TrayBarcodePos1() As String
    <DispId(5)> Property TrayBarcodePos2() As String
    <DispId(6)> Property TrayBarcodePos3() As String
    <DispId(7)> Property TrayBarcodePos4() As String
    <DispId(8)> Property TrayBarcodePos5() As String
    <DispId(9)> Property TrayBarcodePos6() As String

    <DispId(10)> Property TrayBarcodePos7() As String
    <DispId(11)> Property TrayBarcodePos8() As String
    <DispId(12)> Property TrayBarcodePos9() As String
    <DispId(13)> Property TrayBarcodePos10() As String
    <DispId(14)> Property TrayBarcodePos11() As String
    <DispId(15)> Property TrayBarcodePos12() As String

    <DispId(16)> Property LotStatus() As LOT_STATUS
    <DispId(17)> Property NetworkSpec() As String
    <DispId(18)> Property ProgName() As String

    <DispId(19)> Property Product() As String
    <DispId(20)> Property Lot3250() As String
    <DispId(21)> Property OperationName() As String
    <DispId(22)> Property HGAPN() As String

    <DispId(23)> Property QtyIn() As Integer
    <DispId(24)> Property STR() As String
    <DispId(25)> Property Line() As String
    <DispId(26)> Property Suspension() As String
    <DispId(27)> Property Type() As String
    <DispId(28)> Property SuspInv() As String
    <DispId(29)> Property SuspBatch() As String
    <DispId(30)> Property OSC() As String
    <DispId(31)> Property LotUsage() As String


End Interface


<Guid("0186E9BF-0000-4E9B-9D17-8FB9B51FADB6"), ClassInterface(ClassInterfaceType.None), ProgId("WDConnectWrapperLib.TrayInfo")>
Public Class TrayInfo
    Implements ITrayInfoClass

    'ctor
    Public Sub New()
    End Sub

    'dtor
    Protected Overrides Sub Finalize()
    End Sub



    Private _traybarcode As String = ""
    Public Property TrayBarcode As String Implements ITrayInfoClass.TrayBarcode
        Get
            Return _traybarcode
        End Get
        Set(ByVal value As String)
            _traybarcode = value
        End Set
    End Property



    Private _bInProcessLot As Boolean = False
    Public Property IsInProcessLot As Boolean Implements ITrayInfoClass.IsInProcessLot
        Get
            Return _bInProcessLot
        End Get
        Set(ByVal value As Boolean)
            _bInProcessLot = value
        End Set
    End Property



    Private _lotnumber As String = ""
    Public Property LotNumber As String Implements ITrayInfoClass.LotNumber
        Get
            Return _lotnumber
        End Get
        Set(ByVal value As String)
            _lotnumber = value

        End Set
    End Property



    Private _traybarcodePos1 As String = ""
    Public Property TrayBarcodePos1 As String Implements ITrayInfoClass.TrayBarcodePos1
        Get
            Return _traybarcodePos1
        End Get
        Set(ByVal value As String)
            _traybarcodePos1 = value
        End Set
    End Property


    Private _traybarcodePos2 As String = ""
    Public Property TrayBarcodePos2 As String Implements ITrayInfoClass.TrayBarcodePos2
        Get
            Return _traybarcodePos2
        End Get
        Set(ByVal value As String)
            _traybarcodePos2 = value
        End Set
    End Property


    Private _traybarcodePos3 As String = ""
    Public Property TrayBarcodePos3 As String Implements ITrayInfoClass.TrayBarcodePos3
        Get
            Return _traybarcodePos3
        End Get
        Set(ByVal value As String)
            _traybarcodePos3 = value
        End Set
    End Property


    Private _traybarcodePos4 As String = ""
    Public Property TrayBarcodePos4 As String Implements ITrayInfoClass.TrayBarcodePos4
        Get
            Return _traybarcodePos4
        End Get
        Set(ByVal value As String)
            _traybarcodePos4 = value
        End Set
    End Property


    Private _traybarcodePos5 As String = ""
    Public Property TrayBarcodePos5 As String Implements ITrayInfoClass.TrayBarcodePos5
        Get
            Return _traybarcodePos5
        End Get
        Set(ByVal value As String)
            _traybarcodePos5 = value
        End Set
    End Property


    Private _traybarcodePos6 As String = ""
    Public Property TrayBarcodePos6 As String Implements ITrayInfoClass.TrayBarcodePos6
        Get
            Return _traybarcodePos6
        End Get
        Set(ByVal value As String)
            _traybarcodePos6 = value
        End Set
    End Property


    Private _traybarcodePos7 As String = ""
    Public Property TrayBarcodePos7 As String Implements ITrayInfoClass.TrayBarcodePos7
        Get
            Return _traybarcodePos7
        End Get
        Set(ByVal value As String)
            _traybarcodePos7 = value
        End Set
    End Property


    Private _traybarcodePos8 As String = ""
    Public Property TrayBarcodePos8 As String Implements ITrayInfoClass.TrayBarcodePos8
        Get
            Return _traybarcodePos8
        End Get
        Set(ByVal value As String)
            _traybarcodePos8 = value
        End Set
    End Property


    Private _traybarcodePos9 As String = ""
    Public Property TrayBarcodePos9 As String Implements ITrayInfoClass.TrayBarcodePos9
        Get
            Return _traybarcodePos9
        End Get
        Set(ByVal value As String)
            _traybarcodePos9 = value
        End Set
    End Property


    Private _traybarcodePos10 As String = ""
    Public Property TrayBarcodePos10 As String Implements ITrayInfoClass.TrayBarcodePos10
        Get
            Return _traybarcodePos10
        End Get
        Set(ByVal value As String)
            _traybarcodePos10 = value
        End Set
    End Property


    Private _traybarcodePos11 As String = ""
    Public Property TrayBarcodePos11 As String Implements ITrayInfoClass.TrayBarcodePos11
        Get
            Return _traybarcodePos11
        End Get
        Set(ByVal value As String)
            _traybarcodePos11 = value
        End Set
    End Property


    Private _traybarcodePos12 As String = ""
    Public Property TrayBarcodePos12 As String Implements ITrayInfoClass.TrayBarcodePos12
        Get
            Return _traybarcodePos12
        End Get
        Set(ByVal value As String)
            _traybarcodePos12 = value
        End Set
    End Property


    Private _nLotStatus As LOT_STATUS = LOT_STATUS.CAN_ADJUST
    Public Property LotStatus As LOT_STATUS Implements ITrayInfoClass.LotStatus
        Get
            Return _nLotStatus
        End Get
        Set(ByVal value As LOT_STATUS)
            _nLotStatus = value
        End Set
    End Property


    Private _networkSpec As String = ""
    Public Property NetworkSpec As String Implements ITrayInfoClass.NetworkSpec
        Get
            Return _networkSpec
        End Get
        Set(ByVal value As String)
            _networkSpec = value
        End Set
    End Property


    Private _progName As String = ""
    Public Property ProgName As String Implements ITrayInfoClass.ProgName
        Get
            Return _progName
        End Get
        Set(ByVal value As String)
            _progName = value
        End Set
    End Property


    Private _product As String = ""
    Public Property Product As String Implements ITrayInfoClass.Product
        Get
            Return _product
        End Get
        Set(ByVal value As String)
            _product = value
        End Set
    End Property


    Private _lot3250 As String = ""
    Public Property Lot3250 As String Implements ITrayInfoClass.Lot3250
        Get
            Return _lot3250
        End Get
        Set(ByVal value As String)
            _lot3250 = value
        End Set
    End Property


    Private _operationName As String = ""
    Public Property OperationName As String Implements ITrayInfoClass.OperationName
        Get
            Return _operationName
        End Get
        Set(ByVal value As String)
            _operationName = value
        End Set
    End Property


    Private _hgaPN As String = ""
    Public Property HGAPN As String Implements ITrayInfoClass.HGAPN
        Get
            Return _hgaPN
        End Get
        Set(ByVal value As String)
            _hgaPN = value
        End Set
    End Property


    Private _qtyIn As Integer = 0
    Public Property QtyIn As Integer Implements ITrayInfoClass.QtyIn
        Get
            Return _qtyIn
        End Get
        Set(ByVal value As Integer)
            _qtyIn = value
        End Set
    End Property


    Private _str As String = ""
    Public Property STR As String Implements ITrayInfoClass.STR
        Get
            Return _str
        End Get
        Set(ByVal value As String)
            _str = value
        End Set
    End Property


    Private _line As String = ""
    Public Property Line As String Implements ITrayInfoClass.Line
        Get
            Return _line
        End Get
        Set(ByVal value As String)
            _line = value
        End Set
    End Property


    Private _suspension As String = ""
    Public Property Suspension As String Implements ITrayInfoClass.Suspension
        Get
            Return _suspension
        End Get
        Set(ByVal value As String)
            _suspension = value
        End Set
    End Property


    Private _type As String = ""
    Public Property Type As String Implements ITrayInfoClass.Type
        Get
            Return _type
        End Get
        Set(ByVal value As String)
            _type = value
        End Set
    End Property


    Private _suspInv As String = ""
    Public Property SuspInv As String Implements ITrayInfoClass.SuspInv
        Get
            Return _suspInv
        End Get
        Set(ByVal value As String)
            _suspInv = value
        End Set
    End Property


    Private _suspBatch As String = ""
    Public Property SuspBatch As String Implements ITrayInfoClass.SuspBatch
        Get
            Return _suspBatch
        End Get
        Set(ByVal value As String)
            _suspBatch = value
        End Set
    End Property


    Private _osc As String = ""
    Public Property OSC As String Implements ITrayInfoClass.OSC
        Get
            Return _osc
        End Get
        Set(ByVal value As String)
            _osc = value
        End Set
    End Property


    Private _lotUsage As String = ""
    Public Property LotUsage As String Implements ITrayInfoClass.LotUsage
        Get
            Return _lotUsage
        End Get
        Set(ByVal value As String)
            _lotUsage = value
        End Set
    End Property


End Class



'***************************************************************************
<Guid("8DF28A05-8136-422E-A4BA-83FBD8F23089"), InterfaceType(ComInterfaceType.InterfaceIsIDispatch)>
Public Interface IProcessHGAClass

    <DispId(1)> Property Nest() As String
    <DispId(2)> Property PositionOnNest() As Integer
    <DispId(3)> Property PositionOnTray() As Integer
    <DispId(4)> Property PartNo() As Integer
    <DispId(5)> Property ZHeight() As Double
    <DispId(6)> Property IPitch() As Double
    <DispId(7)> Property IRoll() As Double
    <DispId(8)> Property OPitch() As Double
    <DispId(9)> Property ORoll() As Double
    <DispId(10)> Property OStatus() As String
    <DispId(11)> Property Pitch() As Double
    <DispId(12)> Property Roll() As Double
    <DispId(13)> Property Status() As String
    <DispId(14)> Property MeasuredPitchBFADJ() As Double
    <DispId(15)> Property MeasuredPitchAFADJ() As Double
    <DispId(16)> Property CompPitchBFADJ() As Double
    <DispId(17)> Property CompPitchAFADJ() As Double
    <DispId(18)> Property RollBFADJ() As Double
    <DispId(19)> Property RollAFADJ() As Double
    <DispId(20)> Property AdjMarking() As String
    <DispId(21)> Property ZHeightBFADJ() As Double
    <DispId(22)> Property ZHeightAFADJ() As Double
    <DispId(23)> Property VisionX() As Double
    <DispId(24)> Property VisionY() As Double
    <DispId(25)> Property VisionA() As Double
    <DispId(26)> Property MachineDate() As String
    <DispId(27)> Property MachineTime() As String
    <DispId(28)> Property OCR() As String
    <DispId(29)> Property SPCSamplingMarking() As String

End Interface


<Guid("E2554AB4-207F-481E-B554-766D065E352F"), ClassInterface(ClassInterfaceType.None), ProgId("WDConnectWrapperLib.ProcessHGA")>
Public Class ProcessHGA
    Implements IProcessHGAClass

    'ctor
    Public Sub New()
        _nest = ""
        _positionOnNest = 0
        _positionOnTray = 0

        _zHeight = 0.0
        _iPitch = 0.0
        _iRoll = 0.0
        _oPitch = 0.0
        _oRoll = 0.0
        _pitch = 0.0
        _roll = 0.0
        _status = ""
        _measuredPitchBFADJ = 0.0
        _measuredPitchAFADJ = 0.0
        _compPitchBFADJ = 0.0
        _compPitchAFADJ = 0.0
        _rollBFADJ = 0.0
        _rollAFADJ = 0.0
        _adjMarking = ""
        _zHeightBFADJ = 0.0
        _zHeightAFADJ = 0.0
        _visionX = 0.0
        _visionY = 0.0
        _visionA = 0.0
    End Sub

    'dtor
    Protected Overrides Sub Finalize()
    End Sub



    Private _nest As String = ""
    Public Property Nest As String Implements IProcessHGAClass.Nest
        Get
            Return _nest
        End Get
        Set(ByVal value As String)
            _nest = value
        End Set
    End Property



    Private _positionOnNest As Integer = 0
    Public Property PositionOnNest As Integer Implements IProcessHGAClass.PositionOnNest
        Get
            Return _positionOnNest
        End Get
        Set(ByVal value As Integer)
            _positionOnNest = value
        End Set
    End Property



    Private _positionOnTray As Integer = 0
    Public Property PositionOnTray As Integer Implements IProcessHGAClass.PositionOnTray
        Get
            Return _positionOnTray
        End Get
        Set(ByVal value As Integer)
            _positionOnTray = value
        End Set
    End Property



    Private _partNo As Integer = 0
    Public Property PartNo As Integer Implements IProcessHGAClass.PartNo
        Get
            Return _partNo
        End Get
        Set(ByVal value As Integer)
            _partNo = value
        End Set
    End Property



    Private _zHeight As Double = 0.0
    Public Property ZHeight As Double Implements IProcessHGAClass.ZHeight
        Get
            Return _zHeight
        End Get
        Set(ByVal value As Double)
            _zHeight = value
        End Set
    End Property


    Private _iPitch As Double = 0.0
    Public Property IPitch As Double Implements IProcessHGAClass.IPitch
        Get
            Return _iPitch
        End Get
        Set(ByVal value As Double)
            _iPitch = value
        End Set
    End Property



    Private _iRoll As Double = 0.0
    Public Property IRoll As Double Implements IProcessHGAClass.IRoll
        Get
            Return _iRoll
        End Get
        Set(ByVal value As Double)
            _iRoll = value
        End Set
    End Property


    Private _oPitch As Double = 0.0
    Public Property OPitch As Double Implements IProcessHGAClass.OPitch
        Get
            Return _oPitch
        End Get
        Set(ByVal value As Double)
            _oPitch = value
        End Set
    End Property



    Private _oRoll As Double = 0.0
    Public Property ORoll As Double Implements IProcessHGAClass.ORoll
        Get
            Return _oRoll
        End Get
        Set(ByVal value As Double)
            _oRoll = value
        End Set
    End Property



    Private _oStatus As String = ""
    Public Property OStatus As String Implements IProcessHGAClass.OStatus
        Get
            Return _oStatus
        End Get
        Set(ByVal value As String)
            _oStatus = value
        End Set
    End Property



    Private _pitch As Double = 0.0
    Public Property Pitch As Double Implements IProcessHGAClass.Pitch
        Get
            Return _pitch
        End Get
        Set(ByVal value As Double)
            _pitch = value
        End Set
    End Property



    Private _roll As Double = 0.0
    Public Property Roll As Double Implements IProcessHGAClass.Roll
        Get
            Return _roll
        End Get
        Set(ByVal value As Double)
            _roll = value
        End Set
    End Property



    Private _status As String = ""
    Public Property Status As String Implements IProcessHGAClass.Status
        Get
            Return _status
        End Get
        Set(ByVal value As String)
            _status = value
        End Set
    End Property



    Private _measuredPitchBFADJ As Double = 0.0
    Public Property MeasuredPitchBFADJ As Double Implements IProcessHGAClass.MeasuredPitchBFADJ
        Get
            Return _measuredPitchBFADJ
        End Get
        Set(ByVal value As Double)
            _measuredPitchBFADJ = value
        End Set
    End Property



    Private _measuredPitchAFADJ As Double = 0.0
    Public Property MeasuredPitchAFADJ As Double Implements IProcessHGAClass.MeasuredPitchAFADJ
        Get
            Return _measuredPitchAFADJ
        End Get
        Set(ByVal value As Double)
            _measuredPitchAFADJ = value
        End Set
    End Property



    Private _compPitchBFADJ As Double = 0.0
    Public Property CompPitchBFADJ As Double Implements IProcessHGAClass.CompPitchBFADJ
        Get
            Return _compPitchBFADJ
        End Get
        Set(ByVal value As Double)
            _compPitchBFADJ = value
        End Set
    End Property



    Private _compPitchAFADJ As Double = 0.0
    Public Property CompPitchAFADJ As Double Implements IProcessHGAClass.CompPitchAFADJ
        Get
            Return _compPitchAFADJ
        End Get
        Set(ByVal value As Double)
            _compPitchAFADJ = value
        End Set
    End Property



    Private _rollBFADJ As Double = 0.0
    Public Property RollBFADJ As Double Implements IProcessHGAClass.RollBFADJ
        Get
            Return _rollBFADJ
        End Get
        Set(ByVal value As Double)
            _rollBFADJ = value
        End Set
    End Property



    Private _rollAFADJ As Double = 0.0
    Public Property RollAFADJ As Double Implements IProcessHGAClass.RollAFADJ
        Get
            Return _rollAFADJ
        End Get
        Set(ByVal value As Double)
            _rollAFADJ = value
        End Set
    End Property



    Private _adjMarking As String = ""
    Public Property AdjMarking As String Implements IProcessHGAClass.AdjMarking
        Get
            Return _adjMarking
        End Get
        Set(ByVal value As String)
            _adjMarking = value
        End Set
    End Property



    Private _zHeightBFADJ As Double = 0.0
    Public Property ZHeightBFADJ As Double Implements IProcessHGAClass.ZHeightBFADJ
        Get
            Return _zHeightBFADJ
        End Get
        Set(ByVal value As Double)
            _zHeightBFADJ = value
        End Set
    End Property



    Private _zHeightAFADJ As Double = 0.0
    Public Property ZHeightAFADJ As Double Implements IProcessHGAClass.ZHeightAFADJ
        Get
            Return _zHeightAFADJ
        End Get
        Set(ByVal value As Double)
            _zHeightAFADJ = value
        End Set
    End Property



    Private _visionX As Double = 0.0
    Public Property VisionX As Double Implements IProcessHGAClass.VisionX
        Get
            Return _visionX
        End Get
        Set(ByVal value As Double)
            _visionX = value
        End Set
    End Property



    Private _visionY As Double = 0.0
    Public Property VisionY As Double Implements IProcessHGAClass.VisionY
        Get
            Return _visionY
        End Get
        Set(ByVal value As Double)
            _visionY = value
        End Set
    End Property



    Private _visionA As Double = 0.0
    Public Property VisionA As Double Implements IProcessHGAClass.VisionA
        Get
            Return _visionA
        End Get
        Set(ByVal value As Double)
            _visionA = value
        End Set
    End Property



    Private _mDate As String = ""
    Public Property MachineDate As String Implements IProcessHGAClass.MachineDate
        Get
            Return _mDate
        End Get
        Set(ByVal value As String)
            _mDate = value
        End Set
    End Property



    Private _mTime As String = ""
    Public Property MachineTime As String Implements IProcessHGAClass.MachineTime
        Get
            Return _mTime
        End Get
        Set(ByVal value As String)
            _mTime = value
        End Set
    End Property



    Private _ocr As String = ""
    Public Property OCR As String Implements IProcessHGAClass.OCR
        Get
            Return _ocr
        End Get
        Set(ByVal value As String)
            _ocr = value
        End Set
    End Property



    Private _spcSamplingMarking As String = ""
    Public Property SPCSamplingMarking As String Implements IProcessHGAClass.SPCSamplingMarking
        Get
            Return _spcSamplingMarking
        End Get
        Set(ByVal value As String)
            _spcSamplingMarking = value
        End Set
    End Property


End Class



'***************************************************************************
<Guid("905709C1-7D29-4155-8BBD-2EC67CDC74CA"), InterfaceType(ComInterfaceType.InterfaceIsIDispatch)>
Public Interface IAlarmClass
    <DispId(1)> Property AlID() As Integer
    <DispId(2)> Property AlText() As String
    <DispId(3)> Property AlDescription() As String

End Interface


<Guid("D211C2F6-36A7-4938-A4B5-55AA1A59E2A6"), ClassInterface(ClassInterfaceType.None), ProgId("WDConnectWrapperLib.Alarm")>
Public Class Alarm
    Implements IAlarmClass


    'ctor
    Public Sub New()
        _alid = 0
        _altext = ""
        _aldescription = ""
    End Sub



    Private _alid As Integer = 0
    Public Property AlID As Integer Implements IAlarmClass.AlID
        Get
            Return _alid
        End Get
        Set(ByVal value As Integer)
            _alid = value
        End Set
    End Property



    Private _altext As String = ""
    Public Property AlText As String Implements IAlarmClass.AlText
        Get
            Return _altext
        End Get
        Set(ByVal value As String)
            _altext = value
        End Set
    End Property



    Private _aldescription As String = ""
    Public Property AlDescription As String Implements IAlarmClass.AlDescription
        Get
            Return _aldescription
        End Get
        Set(ByVal value As String)
            _aldescription = value
        End Set
    End Property


End Class



'***************************************************************************
<Guid("5604491D-964F-4388-B201-4C4720401BA4"), InterfaceType(ComInterfaceType.InterfaceIsIDispatch)>
Public Interface IPkgSettingClass
    <DispId(1)> Property PitchOffset() As Double
    <DispId(2)> Property RollOffset() As Double
    <DispId(3)> Property ZHeightOffset() As Double
    <DispId(4)> Property PitchUSL() As Double
    <DispId(5)> Property PitchLSL() As Double
    <DispId(6)> Property RollUSL() As Double
    <DispId(7)> Property RollLSL() As Double
    <DispId(8)> Property ZhtTarget() As Double
    <DispId(9)> Property PitchSensitivity() As Double
    <DispId(10)> Property PRAdjAcceptTol() As Double
    <DispId(11)> Property PitchAdjCL() As Double
    <DispId(12)> Property PitchAdjUCL() As Double
    <DispId(13)> Property PitchAdjLCL() As Double
    <DispId(14)> Property RollAdjCL() As Double
    <DispId(15)> Property RollAdjUCL() As Double
    <DispId(16)> Property RollAdjLCL() As Double


End Interface


<Guid("E8446D5C-599B-4F1A-8A24-63DD2A6CB006"), ClassInterface(ClassInterfaceType.None), ProgId("WDConnectWrapperLib.PkgSetting")>
Public Class PkgSetting
    Implements IPkgSettingClass


    'ctor
    Public Sub New()
    End Sub



    Private _pitchOffset As Double = 0.0
    Public Property PitchOffset As Double Implements IPkgSettingClass.PitchOffset
        Get
            Return _pitchOffset
        End Get
        Set(ByVal value As Double)
            _pitchOffset = value
        End Set
    End Property



    Private _rollOffset As Double = 0.0
    Public Property RollOffset As Double Implements IPkgSettingClass.RollOffset
        Get
            Return _rollOffset
        End Get
        Set(ByVal value As Double)
            _rollOffset = value
        End Set
    End Property



    Private _zHeightOffset As Double = 0.0
    Public Property ZHeightOffset As Double Implements IPkgSettingClass.ZHeightOffset
        Get
            Return _zHeightOffset
        End Get
        Set(ByVal value As Double)
            _zHeightOffset = value
        End Set
    End Property



    Private _pitchUSL As Double = 0.0
    Public Property PitchUSL As Double Implements IPkgSettingClass.PitchUSL
        Get
            Return _pitchUSL
        End Get
        Set(ByVal value As Double)
            _pitchUSL = value
        End Set
    End Property


    Private _pitchLSL As Double = 0.0
    Public Property PitchLSL As Double Implements IPkgSettingClass.PitchLSL
        Get
            Return _pitchLSL
        End Get
        Set(ByVal value As Double)
            _pitchLSL = value
        End Set
    End Property


    Private _rollUSL As Double = 0.0
    Public Property RollUSL As Double Implements IPkgSettingClass.RollUSL
        Get
            Return _rollUSL
        End Get
        Set(ByVal value As Double)
            _rollUSL = value
        End Set
    End Property


    Private _rollLSL As Double = 0.0
    Public Property RollLSL As Double Implements IPkgSettingClass.RollLSL
        Get
            Return _rollLSL
        End Get
        Set(ByVal value As Double)
            _rollLSL = value
        End Set
    End Property



    Private _zHtTarget As Double = 0.0
    Public Property ZhtTarget As Double Implements IPkgSettingClass.ZhtTarget
        Get
            Return _zHtTarget
        End Get
        Set(ByVal value As Double)
            _zHtTarget = value
        End Set
    End Property



    Private _pitchSensitivity As Double = 0.0
    Public Property PitchSensitivity As Double Implements IPkgSettingClass.PitchSensitivity
        Get
            Return _pitchSensitivity
        End Get
        Set(ByVal value As Double)
            _pitchSensitivity = value
        End Set
    End Property



    Private _prAdjAcceptTol As Double = 0.0
    Public Property PRAdjAcceptTol As Double Implements IPkgSettingClass.PRAdjAcceptTol
        Get
            Return _prAdjAcceptTol
        End Get
        Set(ByVal value As Double)
            _prAdjAcceptTol = value
        End Set
    End Property



    Private _pitchAdjCL As Double = 0.0
    Public Property PitchAdjCL As Double Implements IPkgSettingClass.PitchAdjCL
        Get
            Return _pitchAdjCL
        End Get
        Set(ByVal value As Double)
            _pitchAdjCL = value
        End Set
    End Property



    Private _pitchAdjUCL As Double = 0.0
    Public Property PitchAdjUCL As Double Implements IPkgSettingClass.PitchAdjUCL
        Get
            Return _pitchAdjUCL
        End Get
        Set(ByVal value As Double)
            _pitchAdjUCL = value
        End Set
    End Property



    Private _pitchAdjLCL As Double = 0.0
    Public Property PitchAdjLCL As Double Implements IPkgSettingClass.PitchAdjLCL
        Get
            Return _pitchAdjLCL
        End Get
        Set(ByVal value As Double)
            _pitchAdjLCL = value
        End Set
    End Property



    Private _rollAdjCL As Double = 0.0
    Public Property RollAdjCL As Double Implements IPkgSettingClass.RollAdjCL
        Get
            Return _rollAdjCL
        End Get
        Set(ByVal value As Double)
            _rollAdjCL = value
        End Set
    End Property



    Private _rollAdjUCL As Double = 0.0
    Public Property RollAdjUCL As Double Implements IPkgSettingClass.RollAdjUCL
        Get
            Return _rollAdjUCL
        End Get
        Set(ByVal value As Double)
            _rollAdjUCL = value
        End Set
    End Property



    Private _rollAdjLCL As Double = 0.0
    Public Property RollAdjLCL As Double Implements IPkgSettingClass.RollAdjLCL
        Get
            Return _rollAdjLCL
        End Get
        Set(ByVal value As Double)
            _rollAdjLCL = value
        End Set
    End Property



End Class



'*********************