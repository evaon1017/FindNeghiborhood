<%@ Page Language="C#" AutoEventWireup="true" Async="true" CodeBehind="Index.aspx.cs" Inherits="FindNeghiborhood.Index" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta name="viewport" content="width=device-width,initial-scale=1, maximum-scale=1, user-scalable=yes" />
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <script src="node_modules/jquery/dist/jquery.min.js"></script>
    <script src="node_modules/web3/dist/web3.min.js"></script>
    <style>
        td, th {
            border: 1px solid black;
            padding: 3px;
        }

        .short {
            width: 50px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager runat="server" ID="SM"></asp:ScriptManager>
        <asp:UpdatePanel runat="server">
            <ContentTemplate>
                <h3>目前已登記土地
                    <asp:Literal runat="server" ID="UI_litLandCount" />筆</h3>
            </ContentTemplate>
        </asp:UpdatePanel>
        <span id="msg"></span>
        <div id="wallet-div" style="display:none"><span id="start-connect">我的錢包(點我):</span><span id="my-wallet"></span></div>
        <asp:Button runat="server" ID="UI_btnScanAll" OnClick="UI_btnScanAll_Click" Visible="false" />
        <br />
        <div class="land-share" style="display: none">
            <h3>登記找鄰居(請手動填入地產的token id，還有希望別人如何聯繫你的discord)</h3>
            <ul>
                <li>土地token id:<br />
                    <asp:TextBox runat="server" ID="txtRegister" ClientIDMode="Static" Width="90%" TextMode="MultiLine" Rows="5" />
                    <br />
                    如果有多個土地，請一行一個id
                </li>
                <li style="margin-top: 30px;">discord id:<asp:TextBox runat="server" ID="discordId" Width="90%" /></li>
            </ul>
            <input type="button" id="btn-submit" value="送出" />
        </div>
        <div class="land-share" style="display: none">
            <asp:UpdatePanel runat="server" ID="UP" UpdateMode="Conditional" ChildrenAsTriggers="false">
                <ContentTemplate>
                    <asp:HiddenField runat="server" ID="UI_hidAccount" />
                    <asp:LinkButton runat="server" ID="UI_btnReload" OnClick="UI_btnReload_Click" />
                    我的已列出地產：<a href="Map.aspx" target="_blank" >看地圖</a><br />
                    <table cellspacing="0" cellpadding="0" border="1">
                        <tr>
                            <th>City</th>
                            <th>Town</th>
                            <th>Cord</th>
                            <th>我的鄰居</th>
                        </tr>
                        <asp:Repeater runat="server" ID="UI_rep" ItemType="System.String[]">
                            <ItemTemplate>
                                <tr>
                                    <td><%# Item[0] %></td>
                                    <td><%# Item[1] %></td>
                                    <td><%# Item[2] %></td>
                                    <td><%# Item[3] %></td>
                                </tr>
                            </ItemTemplate>
                        </asp:Repeater>
                    </table>
                </ContentTemplate>
            </asp:UpdatePanel>
            <asp:HiddenField runat="server" ID="UI_hid" />
            <asp:LinkButton runat="server" ID="UI_btnSend" OnClick="UI_btnSend_Click" />

        </div>
    </form>
    <script>
        var abi = [
            {
                "anonymous": false,
                "inputs": [
                    {
                        "indexed": true,
                        "internalType": "address",
                        "name": "owner",
                        "type": "address"
                    },
                    {
                        "indexed": true,
                        "internalType": "address",
                        "name": "approved",
                        "type": "address"
                    },
                    {
                        "indexed": true,
                        "internalType": "uint256",
                        "name": "tokenId",
                        "type": "uint256"
                    }
                ],
                "name": "Approval",
                "type": "event"
            },
            {
                "anonymous": false,
                "inputs": [
                    {
                        "indexed": true,
                        "internalType": "address",
                        "name": "owner",
                        "type": "address"
                    },
                    {
                        "indexed": true,
                        "internalType": "address",
                        "name": "operator",
                        "type": "address"
                    },
                    {
                        "indexed": false,
                        "internalType": "bool",
                        "name": "approved",
                        "type": "bool"
                    }
                ],
                "name": "ApprovalForAll",
                "type": "event"
            },
            {
                "anonymous": false,
                "inputs": [
                    {
                        "indexed": true,
                        "internalType": "address",
                        "name": "from",
                        "type": "address"
                    },
                    {
                        "indexed": true,
                        "internalType": "address",
                        "name": "to",
                        "type": "address"
                    },
                    {
                        "indexed": true,
                        "internalType": "uint256",
                        "name": "tokenId",
                        "type": "uint256"
                    }
                ],
                "name": "Transfer",
                "type": "event"
            },
            {
                "inputs": [
                    {
                        "internalType": "address",
                        "name": "to",
                        "type": "address"
                    },
                    {
                        "internalType": "uint256",
                        "name": "tokenId",
                        "type": "uint256"
                    }
                ],
                "name": "approve",
                "outputs": [],
                "stateMutability": "nonpayable",
                "type": "function"
            },
            {
                "inputs": [
                    {
                        "internalType": "address",
                        "name": "owner",
                        "type": "address"
                    }
                ],
                "name": "balanceOf",
                "outputs": [
                    {
                        "internalType": "uint256",
                        "name": "balance",
                        "type": "uint256"
                    }
                ],
                "stateMutability": "view",
                "type": "function"
            },
            {
                "inputs": [
                    {
                        "internalType": "uint256",
                        "name": "tokenId",
                        "type": "uint256"
                    }
                ],
                "name": "getApproved",
                "outputs": [
                    {
                        "internalType": "address",
                        "name": "operator",
                        "type": "address"
                    }
                ],
                "stateMutability": "view",
                "type": "function"
            },
            {
                "inputs": [
                    {
                        "internalType": "address",
                        "name": "owner",
                        "type": "address"
                    },
                    {
                        "internalType": "address",
                        "name": "operator",
                        "type": "address"
                    }
                ],
                "name": "isApprovedForAll",
                "outputs": [
                    {
                        "internalType": "bool",
                        "name": "",
                        "type": "bool"
                    }
                ],
                "stateMutability": "view",
                "type": "function"
            },
            {
                "inputs": [],
                "name": "name",
                "outputs": [
                    {
                        "internalType": "string",
                        "name": "",
                        "type": "string"
                    }
                ],
                "stateMutability": "view",
                "type": "function"
            },
            {
                "inputs": [
                    {
                        "internalType": "uint256",
                        "name": "tokenId",
                        "type": "uint256"
                    }
                ],
                "name": "ownerOf",
                "outputs": [
                    {
                        "internalType": "address",
                        "name": "owner",
                        "type": "address"
                    }
                ],
                "stateMutability": "view",
                "type": "function"
            },
            {
                "inputs": [
                    {
                        "internalType": "address",
                        "name": "from",
                        "type": "address"
                    },
                    {
                        "internalType": "address",
                        "name": "to",
                        "type": "address"
                    },
                    {
                        "internalType": "uint256",
                        "name": "tokenId",
                        "type": "uint256"
                    }
                ],
                "name": "safeTransferFrom",
                "outputs": [],
                "stateMutability": "nonpayable",
                "type": "function"
            },
            {
                "inputs": [
                    {
                        "internalType": "address",
                        "name": "from",
                        "type": "address"
                    },
                    {
                        "internalType": "address",
                        "name": "to",
                        "type": "address"
                    },
                    {
                        "internalType": "uint256",
                        "name": "tokenId",
                        "type": "uint256"
                    },
                    {
                        "internalType": "bytes",
                        "name": "data",
                        "type": "bytes"
                    }
                ],
                "name": "safeTransferFrom",
                "outputs": [],
                "stateMutability": "nonpayable",
                "type": "function"
            },
            {
                "inputs": [
                    {
                        "internalType": "address",
                        "name": "operator",
                        "type": "address"
                    },
                    {
                        "internalType": "bool",
                        "name": "_approved",
                        "type": "bool"
                    }
                ],
                "name": "setApprovalForAll",
                "outputs": [],
                "stateMutability": "nonpayable",
                "type": "function"
            },
            {
                "inputs": [
                    {
                        "internalType": "bytes4",
                        "name": "interfaceId",
                        "type": "bytes4"
                    }
                ],
                "name": "supportsInterface",
                "outputs": [
                    {
                        "internalType": "bool",
                        "name": "",
                        "type": "bool"
                    }
                ],
                "stateMutability": "view",
                "type": "function"
            },
            {
                "inputs": [],
                "name": "symbol",
                "outputs": [
                    {
                        "internalType": "string",
                        "name": "",
                        "type": "string"
                    }
                ],
                "stateMutability": "view",
                "type": "function"
            },
            {
                "inputs": [
                    {
                        "internalType": "uint256",
                        "name": "index",
                        "type": "uint256"
                    }
                ],
                "name": "tokenByIndex",
                "outputs": [
                    {
                        "internalType": "uint256",
                        "name": "",
                        "type": "uint256"
                    }
                ],
                "stateMutability": "view",
                "type": "function"
            },
            {
                "inputs": [
                    {
                        "internalType": "address",
                        "name": "owner",
                        "type": "address"
                    },
                    {
                        "internalType": "uint256",
                        "name": "index",
                        "type": "uint256"
                    }
                ],
                "name": "tokenOfOwnerByIndex",
                "outputs": [
                    {
                        "internalType": "uint256",
                        "name": "",
                        "type": "uint256"
                    }
                ],
                "stateMutability": "view",
                "type": "function"
            },
            {
                "inputs": [
                    {
                        "internalType": "uint256",
                        "name": "tokenId",
                        "type": "uint256"
                    }
                ],
                "name": "tokenURI",
                "outputs": [
                    {
                        "internalType": "string",
                        "name": "",
                        "type": "string"
                    }
                ],
                "stateMutability": "view",
                "type": "function"
            },
            {
                "inputs": [],
                "name": "totalSupply",
                "outputs": [
                    {
                        "internalType": "uint256",
                        "name": "",
                        "type": "uint256"
                    }
                ],
                "stateMutability": "view",
                "type": "function"
            },
            {
                "inputs": [
                    {
                        "internalType": "address",
                        "name": "from",
                        "type": "address"
                    },
                    {
                        "internalType": "address",
                        "name": "to",
                        "type": "address"
                    },
                    {
                        "internalType": "uint256",
                        "name": "tokenId",
                        "type": "uint256"
                    }
                ],
                "name": "transferFrom",
                "outputs": [],
                "stateMutability": "nonpayable",
                "type": "function"
            }
        ];

    </script>
    <script>
        var web3;
        var cc;
        var acc;

        if (typeof (window.ethereum) == 'undefined') {
            var msg = '沒有偵測到安裝的錢包，如果是手機瀏覽，請記得使用錢包的瀏覽器';
            $('#msg').text(msg);
            alert(msg);
        }
        else {
            init();
        }
        
        ethereum.on('chainChanged', () => window.location = window.location.toString());
        ethereum.on('accountChanged', () => window.location = window.location.toString());

        async function init() {
            web3 = new Web3(Web3.givenProvider);

            var chainId = await web3.eth.getChainId();

            if (chainId != 137) {
                var msg = '請切換到Polygon(Matic)網路喔';
                $('#msg').text(msg);
                alert(msg);
                return;
            }

            $('#wallet-div').show();

            var connectButton = document.getElementById('start-connect');
            connectButton.onclick = async function () {
                
                var v = await ethereum.request({ method: 'eth_requestAccounts' });

                var account = await web3.eth.getAccounts();

                if (account.length == 0) {
                    var msg = '請提供至少一個錢包做Connect';
                    $('#msg').text(msg);
                    alert(msg);
                    return;
                }

                acc = account[0];

                $('#<%= this.UI_hidAccount.ClientID %>').val(acc);
                $('#<%= this.UI_btnReload.ClientID %>')[0].click();


                cc = new web3.eth.Contract(abi, '0x82016d4aD050ef4784e282b82A746D3e01dF23BF');

                connectButton.onclick = null;
                connectButton.textContent = '我的錢包:';

                document.getElementById('my-wallet').textContent = acc;

                $('.land-share').show();

                var txtDiscordId = $('#<%= this.discordId.ClientID %>');

                $('#btn-submit').click(async function () {
                    var tokenId = $('#txtRegister').val();
                    var discordId = txtDiscordId.val();

                    if (tokenId.indexOf('\n') > 0) {
                        tokenId = tokenId.split('\n');
                        for (var i = 0; i < tokenId.length; i++) {
                            try {
                                tokenId[i] = parseInt(tokenId[i]);
                            } catch (e) {
                                setTimeout(() => alert('id 是不是打錯了，每一行應該都是純數字喔'), 100);
                                $('#txtRegister').focus();
                                return;
                            }
                        }
                    }
                    else {
                        try {
                            tokenId = parseInt(tokenId);
                        } catch (e) {
                            setTimeout(() => alert('id 是不是打錯了，應該是純數字喔'), 100);
                            $('#txtRegister').focus();
                            return;
                        }

                        tokenId = [tokenId];
                    }

                    for (var i = 0; i < tokenId.length; i++) {
                        var t = tokenId[i];
                        var tokenOwner = await cc.methods.ownerOf(t).call();
                        if (tokenOwner != acc) {
                            setTimeout(() => alert('你不是 id: ' + t + ' 土地的擁有者喔'), 100);
                            $('#txtRegister').focus();
                            return;
                        }
                    }

                    if (discordId == null || discordId == '') {
                        setTimeout(() => alert('discord id 忘記填啦'), 100);
                        txtDiscordId.focus();
                        return;
                    }

                    $('#<%= this.UI_hid.ClientID %>').val(JSON.stringify({
                        address: acc,
                        token: tokenId,
                        discord: discordId
                    }));

                    await $('#<%= this.UI_btnSend.ClientID %>')[0].click();

                    setTimeout(() => alert('ok'), 100);

                    $('#txt-register').val(null);
                    $('#txt-register').focus();
                });
            };
        }
    </script>
</body>
</html>
