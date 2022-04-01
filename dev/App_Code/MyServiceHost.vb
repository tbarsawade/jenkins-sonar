Imports Microsoft.VisualBasic
Imports System.ServiceModel.Web
Imports System.ServiceModel.Activation
Imports System.ServiceModel
Imports System.ServiceModel.Channels

Public Class MyServiceHost
    Inherits WebServiceHost
    Public Sub New()
    End Sub

    Public Sub New(singletonInstance As Object, ParamArray baseAddresses As Uri())
        MyBase.New(singletonInstance, baseAddresses)
    End Sub

    Public Sub New(serviceType As Type, ParamArray baseAddresses As Uri())
        MyBase.New(serviceType, baseAddresses)
    End Sub


    Protected Overrides Sub OnOpening()
        MyBase.OnOpening()

        If MyBase.Description IsNot Nothing Then
            For Each endpoint In MyBase.Description.Endpoints
                Dim transport = endpoint.Binding.CreateBindingElements().Find(Of TransportBindingElement)()
                If transport IsNot Nothing Then
                    transport.MaxReceivedMessageSize = 5242880
                    transport.MaxBufferPoolSize = 5242880
                    transport.MaxBufferPoolSize = 5242880
                End If
            Next
        End If
    End Sub
End Class
Class MyWebServiceHostFactory
    Inherits WebServiceHostFactory
    Protected Overrides Function CreateServiceHost(serviceType As Type, baseAddresses As Uri()) As ServiceHost
        Return New MyServiceHost(serviceType, baseAddresses)
    End Function
End Class

Public NotInheritable Class MyServiceHostFactory
    Inherits System.ServiceModel.Activation.ServiceHostFactory
    Public Overrides Function CreateServiceHost(constructorString As String, baseAddresses As Uri()) As System.ServiceModel.ServiceHostBase
        Return MyBase.CreateServiceHost(constructorString, baseAddresses)
    End Function

    Protected Overrides Function CreateServiceHost(serviceType As Type, baseAddresses As Uri()) As System.ServiceModel.ServiceHost
        Return New MyServiceHost(serviceType, baseAddresses)
    End Function
End Class
