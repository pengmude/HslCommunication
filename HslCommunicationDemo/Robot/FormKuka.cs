﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HslCommunication.Profinet;
using System.Threading;
using HslCommunication.Robot.KUKA;
using HslCommunication;
using System.Xml.Linq;
using HslCommunicationDemo.DemoControl;

namespace HslCommunicationDemo
{
	public partial class FormKuka : HslFormContent
	{
		public FormKuka( )
		{
			InitializeComponent( );
		}


		private KukaAvarProxyNet kuka = null;
		private AddressExampleControl addressExampleControl;
		private CodeExampleControl codeExampleControl;

		private void FormSiemens_Load( object sender, EventArgs e )
		{
			panel2.Enabled = false;
			
			Language( Program.Language );

			addressExampleControl = new AddressExampleControl( );
			addressExampleControl.SetAddressExample( 
				new DeviceAddressExample[]
				{
					new DeviceAddressExample( "$OV_PRO",    "OV_PRO variable",   false, false, "ReadString / ReadStringAsync" ),
					new DeviceAddressExample( "$OV_JOG",    "OV_JOG variable",   false, false, "ReadString / ReadStringAsync" ),
					new DeviceAddressExample( "$AXIS_ACT",  "AXIS_ACT variable", false, false, "ReadString / ReadStringAsync" ),
					new DeviceAddressExample( "$POS_ACT",   "POS_ACT variable",  false, false, "ReadString / ReadStringAsync" ),
					new DeviceAddressExample( "$MODE_OP",   "MODE_OP variable",  false, false, "ReadString / ReadStringAsync" ),
					new DeviceAddressExample( "$OUT[1]",    "OUT[1] variable",   false, false, "ReadString / ReadStringAsync" ),
				} );
			DemoUtils.AddSpecialFunctionTab( this.tabControl1, addressExampleControl, false, DeviceAddressExample.GetTitle( ) );

			codeExampleControl = new CodeExampleControl( );
			DemoUtils.AddSpecialFunctionTab( this.tabControl1, codeExampleControl, false, CodeExampleControl.GetTitle( ) );

		}

		private void Language( int language )
		{
			if (language == 2)
			{
				Text = "Kuka Robot Demo";

				label1.Text = "Ip:";
				label2.Text = "You need to install a VB program on the robot controller, which comes from github: ";
				label3.Text = "Port:";
				button1.Text = "Connect";
				button2.Text = "Disconnect";
				label6.Text = "address:";
				label7.Text = "result:";

				button_read_string.Text = "r-string";


				label10.Text = "Address:";
				label9.Text = "Value:";

				button14.Text = "w-string";

				groupBox1.Text = "Single Data Read test";
				groupBox2.Text = "Single Data Write test";
			}
		}

		private void FormSiemens_FormClosing( object sender, FormClosingEventArgs e )
		{
		}


		#region Connect And Close


		private async void button1_Click( object sender, EventArgs e )
		{
			if(!int.TryParse(textBox2.Text,out int port))
			{
				MessageBox.Show( "端口输入格式不正确！" );
				return;
			}
			
			kuka?.ConnectClose( );
			kuka = new KukaAvarProxyNet( textBox1.Text, port );
			kuka.LogNet = this.LogNet;

			try
			{
				OperateResult connect = await kuka.ConnectServerAsync( );
				if (connect.IsSuccess)
				{
					MessageBox.Show( HslCommunication.StringResources.Language.ConnectedSuccess );
					button2.Enabled = true;
					button1.Enabled = false;
					panel2.Enabled = true;

					// 设置代码示例
					codeExampleControl.SetCodeText( "robot", kuka );
				}
				else
				{
					MessageBox.Show( HslCommunication.StringResources.Language.ConnectedFailed + connect.Message );
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show( ex.Message );
			}
		}

		private async void button2_Click( object sender, EventArgs e )
		{
			// 断开连接
			await kuka.ConnectCloseAsync( );
			button2.Enabled = false;
			button1.Enabled = true;
			panel2.Enabled = false;
		}

		#endregion

		#region 单数据读取测试
		

		private async void button_read_string_Click( object sender, EventArgs e )
		{
			// 读取字符串
			DemoUtils.ReadResultRender( await kuka.ReadStringAsync( textBox3.Text ), textBox3.Text, textBox4 );
		}


		#endregion

		#region 单数据写入测试
		

		private async void button14_Click( object sender, EventArgs e )
		{
			// string写入
			try
			{
				DemoUtils.WriteResultRender( await kuka.WriteAsync( textBox8.Text, textBox7.Text ), textBox8.Text );
			}
			catch (Exception ex)
			{
				MessageBox.Show( ex.Message );
			}
		}

		#endregion

		public override void SaveXmlParameter( XElement element )
		{
			element.SetAttributeValue( DemoDeviceList.XmlIpAddress, textBox1.Text );
			element.SetAttributeValue( DemoDeviceList.XmlPort, textBox2.Text );
		}

		public override void LoadXmlParameter( XElement element )
		{
			base.LoadXmlParameter( element );
			textBox1.Text = element.Attribute( DemoDeviceList.XmlIpAddress ).Value;
			textBox2.Text = element.Attribute( DemoDeviceList.XmlPort ).Value;
		}

		private void userControlHead1_SaveConnectEvent_1( object sender, EventArgs e )
		{
			userControlHead1_SaveConnectEvent( sender, e );
		}

		private void linkLabel1_LinkClicked( object sender, LinkLabelLinkClickedEventArgs e )
		{
			try
			{
				System.Diagnostics.Process.Start( linkLabel1.Text );
			}
			catch (Exception ex)
			{
				HslCommunication.BasicFramework.SoftBasic.ShowExceptionMessage( ex );
			}
		}
	}
	
}
