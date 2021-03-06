﻿/**
 * Copyright 2018 The Nakama Authors
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System.Collections;
using System.Collections.Generic;
using Nakama;
using UnityEngine;

public class SocketDebugger : MonoBehaviour
{
    private const int SendDelaySec = 5;

    private IClient _client = new Client("defaultkey", "127.0.0.1", 7350, false);
    private ISocket _socket;

    private async void Awake()
    {
        var deviceid = SystemInfo.deviceUniqueIdentifier;
        var session = await _client.AuthenticateDeviceAsync(deviceid);

        _socket = _client.CreateWebSocket();
        _socket.Logger = new UnityLogger();
        _socket.Trace = true;

        await _socket.ConnectAsync(session);
    }

    private void Start()
    {
        StartCoroutine(MessageSender());
    }

    private IEnumerator MessageSender()
    {
        yield return new WaitForSeconds(SendDelaySec);
        _socket.RpcAsync("somefunc", string.Empty);
        StartCoroutine(MessageSender());
    }

    private async void OnApplicationQuit()
    {
        if (_socket != null)
        {
            await _socket.DisconnectAsync(false);
        }
    }
}
