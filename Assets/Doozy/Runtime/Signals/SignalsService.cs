﻿// Copyright (c) 2015 - 2021 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using System.Linq;
using Doozy.Runtime.Common.Extensions;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnassignedField.Global

namespace Doozy.Runtime.Signals
{
    public static class SignalsService
    {
        #region Providers

        /// <summary> Providers Database </summary>
        public static readonly List<ISignalProvider> Providers = new List<ISignalProvider>();

        /// <summary> Whenever a ISignalProvider is added, this action gets invoked </summary>
        public static UnityAction<ISignalProvider> OnProviderAdded;

        /// <summary> Whenever a ISignalProvider is removed, this action gets invoked </summary>
        public static UnityAction<ISignalProvider> OnProviderRemoved;

        /// <summary> Add a new SignalProvider to Providers </summary>
        internal static ISignalProvider AddProvider(ISignalProvider provider)
        {
            RemoveNullProviders();
            if (provider == null) return null;
            if (Providers.Contains(provider)) return provider;
            Providers.Add(provider);
            OnProviderAdded?.Invoke(provider);
            return provider;
        }

        /// <summary> Remove the given provider from Providers </summary>
        internal static void RemoveProvider(ISignalProvider provider)
        {
            RemoveNullProviders();
            if (provider == null) return;
            if (!Providers.Contains(provider)) return;
            Providers.Remove(provider);
            OnProviderRemoved?.Invoke(provider);
        }

        internal static void RemoveNullProviders()
        {
            for (int i = Providers.Count - 1; i >= 0; i--)
                if (Providers[i] == null)
                    Providers.RemoveAt(i);
        }

        /// <summary>
        /// Get the provider with the given provider id
        /// If a provider is not found, one will get created
        /// </summary>
        /// <param name="providerId"> Provider id </param>
        /// <param name="signalSource"> If the provider type is Local, then it needs a signalSource to attach to </param>
        public static ISignalProvider GetProvider(ProviderId providerId, GameObject signalSource)
        {
            Type providerType = SignalProvider.GetProviderType(providerId);
            signalSource = providerId.Type == ProviderType.Global ? Signals.instance.gameObject : signalSource;
            if (signalSource == null) throw new NullReferenceException($"{nameof(signalSource)} cannot be null when the {nameof(ProviderType)} is {providerId.Type}");
            Component component = signalSource.GetComponent(providerType) ?? signalSource.AddComponent(providerType);
            // ReSharper disable once SuspiciousTypeConversion.Global
            return (ISignalProvider)component;
        }

        /// <summary>
        /// Get the provider with the given properties
        /// If a provider is not found, one will get created
        /// </summary>
        /// <param name="providerType"> Type of provider (Local or Global) </param>
        /// <param name="providerCategory"> Provider category name </param>
        /// <param name="providerName"> Provider name (from the given category) </param>
        /// <param name="signalSource"> If the provider type is Local, then it needs a signalSource to attach to </param>
        public static ISignalProvider GetProvider(ProviderType providerType, string providerCategory, string providerName, GameObject signalSource) =>
            GetProvider(new ProviderId(providerType, providerCategory, providerName), signalSource);

        /// <summary>
        /// Get the provider associated with the given stream
        /// If a provider is not found, this method returns null
        /// </summary>
        /// <param name="stream"> Signal stream </param>
        public static ISignalProvider GetProvider(SignalStream stream) =>
            Providers.FirstOrDefault(provider => provider.stream == stream);

        #endregion

        #region Streams

        public const string k_TypeCategory = "Type";

        /// <summary> Streams Database </summary>
        public static readonly Dictionary<Guid, SignalStream> Streams = new Dictionary<Guid, SignalStream>();

        /// <summary> Whenever a Stream is added, this action gets invoked </summary>
        public static UnityAction<SignalStream> OnStreamAdded;

        /// <summary> Whenever a Stream is removed, this action gets invoked </summary>
        public static UnityAction<SignalStream> OnStreamRemoved;

        /// <summary> Add a new SignalStream to Streams </summary>
        internal static SignalStream AddStream(SignalStream stream)
        {
            if (stream == null) return null;
            if (Streams.ContainsValue(stream)) return stream;
            Streams.Add(stream.key, stream);
            OnStreamAdded?.Invoke(stream);
            return stream;
        }

        /// <summary> Remove the given stream from Streams </summary>
        internal static void RemoveStream(SignalStream stream)
        {
            if (stream == null) return;
            if (!Streams.ContainsKey(stream.key)) return;
            Streams.Remove(stream.key);
            OnStreamRemoved?.Invoke(stream);
        }

        /// <summary>
        /// Get a new unique stream key (Guid)
        /// <para/> This method makes sure the newly generated Guid is not used by any other registered stream
        /// </summary>
        internal static Guid GetNewStreamKey()
        {
            Guid guid = Guid.Empty;
            bool generateNewId = true;
            while (generateNewId)
            {
                guid = Guid.NewGuid();
                generateNewId = Streams.ContainsKey(guid);
            }

            return guid;
        }

        #region GetStream

        /// <summary> Create a new stream and get a reference to it </summary>
        public static SignalStream GetStream() =>
            AddStream(new SignalStream(GetNewStreamKey()));

        public static SignalStream GetTypeStream(string typeName) =>
            GetStream(typeName, k_TypeCategory);

        /// <summary>
        /// Get the stream with the given streamName and streamCategory.
        /// If not found, this method creates a new stream with the given stream name and category, and returns a reference to it
        /// </summary>
        /// <param name="streamName"> Stream name </param>
        /// <param name="streamCategory"> Stream category</param>
        public static SignalStream GetStream(string streamName, string streamCategory = SignalStream.k_DefaultCategory)
        {
            streamCategory = streamCategory.Trim();
            if (streamCategory.IsNullOrEmpty())
                streamCategory = SignalStream.k_DefaultCategory;

            streamName = streamName.Trim();
            if (streamName.IsNullOrEmpty())
                return GetStream();

            foreach (SignalStream s in Streams.Values
                .Where(s => s.category.Equals(streamCategory))
                .Where(s => s.name.Equals(streamName)))
                return s;

            SignalStream stream = new SignalStream(GetNewStreamKey()).SetCategory(streamCategory).SetName(streamName);
            return AddStream(stream);
        }

        #endregion

        #region FindStream

        /// <summary>
        /// Find the stream with the given stream key.
        /// If not found, this method returns null
        /// </summary>
        /// <param name="streamKey"> Stream key to search for </param>
        public static SignalStream FindStream(Guid streamKey) =>
            Streams.ContainsKey(streamKey) ? Streams[streamKey] : null;

        /// <summary>
        /// Find the stream with the given stream name and category.
        /// If not found, this method returns null </summary>
        /// <param name="streamName"> Stream name </param>
        /// <param name="streamCategory"> Stream category </param>
        public static SignalStream FindStream(string streamName, string streamCategory = SignalStream.k_DefaultCategory)
        {
            streamCategory = streamCategory.Trim();
            if (streamCategory.IsNullOrEmpty())
                streamCategory = SignalStream.k_DefaultCategory;

            streamName = streamName.Trim();
            if (streamName.IsNullOrEmpty())
                return null;

            return Streams.Values
                .Where(s => s.category.Equals(streamCategory))
                .FirstOrDefault(s => s.name.Equals(streamName));
        }

        #endregion

        #region CloseStream

        /// <summary> Close the given stream </summary>
        /// <param name="stream"> Target signal stream </param>
        public static void CloseStream(SignalStream stream)
        {
            stream?.Close();
            RemoveStream(stream);
            RemoveProvider(GetProvider(stream));
        }

        /// <summary> Close the stream associated to the given provider </summary>
        /// <param name="provider"> Target signal provider </param>
        public static void CloseStream(ISignalProvider provider)
        {
            if (provider == null) return;
            if (provider.isConnected)
            {
                CloseStream(provider.stream);
                return;
            }
            RemoveProvider(provider);
        }

        #endregion

        #endregion

        #region SendSignal

        /// <summary> Whenever a Signal is sent, this action gets invoked </summary>
        public static UnityAction<Signal> OnSignal;

        #region Send Signal - using a StreamName and a StreamCategory

        /// <summary> Send a Signal on the stream with the given stream name and category </summary>
        /// <param name="streamName"> Target stream name </param>
        /// <param name="streamCategory"> Target stream category </param>
        /// <param name="message"> Text message used to pass info about this Signal </param>
        public static bool SendSignal(string streamName, string streamCategory, string message) =>
            SendSignal(GetStream(streamName, streamCategory), message);

        /// <summary> Send a Signal on the stream with the given stream name and category, with a reference to the GameObject from where it is sent </summary>
        /// <param name="streamName"> Target stream name </param>
        /// <param name="streamCategory"> Target stream category </param>
        /// <param name="signalSource"> Reference to the GameObject from where this Signal is sent </param>
        /// <param name="message"> Text message used to pass info about this Signal </param>
        public static bool SendSignal(string streamName, string streamCategory, GameObject signalSource, string message = "") =>
            SendSignal(GetStream(streamName, streamCategory), signalSource, message);

        /// <summary> Send a Signal on the stream with the given stream name and category, with a reference to the SignalProvider that sent it </summary>
        /// <param name="streamName"> Target stream name </param>
        /// <param name="streamCategory"> Target stream category </param>
        /// <param name="signalProvider"> Reference to the SignalProvider that sends this Signal </param>
        /// <param name="message"> Text message used to pass info about this Signal </param>
        public static bool SendSignal(string streamName, string streamCategory, SignalProvider signalProvider, string message = "") =>
            SendSignal(GetStream(streamName, streamCategory), signalProvider, message);

        /// <summary> Send a Signal on the stream with the given stream name and category, with a reference to the Object that sent it </summary>
        /// <param name="streamName"> Target stream name </param>
        /// <param name="streamCategory"> Target stream category </param>
        /// <param name="signalSender"> Reference to the Object that sends this Signal </param>
        /// <param name="message"> Text message used to pass info about this Signal </param>
        public static bool SendSignal(string streamName, string streamCategory, Object signalSender, string message = "") =>
            SendSignal(GetStream(streamName, streamCategory), signalSender, message);

        #endregion

        #region Send MetaSignal - using a StreamName and a StreamCategory

        /// <summary> Send a MetaSignal on the stream with the given stream name and category </summary>
        /// <param name="streamName"> Target stream name </param>
        /// <param name="streamCategory"> Target stream category </param>
        /// <param name="signalValue"> Signal value </param>
        /// <param name="message"> Text message used to pass info about this Signal </param>
        public static bool SendSignal<T>(string streamName, string streamCategory, T signalValue, string message = "") =>
            SendSignal(GetStream(streamName, streamCategory), signalValue, message);

        /// <summary> Send a MetaSignal on the stream with the given stream name and category, with a reference to the GameObject from where it is sent </summary>
        /// <param name="streamName"> Target stream name </param>
        /// <param name="streamCategory"> Target stream category </param>
        /// <param name="signalValue"> Signal value </param>
        /// <param name="signalSource"> Reference to the GameObject from where this Signal is sent </param>
        /// <param name="message"> Text message used to pass info about this Signal </param>
        /// <typeparam name="T"> Signal value type </typeparam>
        public static bool SendSignal<T>(string streamName, string streamCategory, T signalValue, GameObject signalSource, string message = "") =>
            SendSignal(GetStream(streamName, streamCategory), signalValue, signalSource, message);

        /// <summary> Send a MetaSignal on the stream with the given stream name and category, with a reference to the SignalProvider that sent it </summary>
        /// <param name="streamName"> Target stream name </param>
        /// <param name="streamCategory"> Target stream category </param>
        /// <param name="signalValue"> Signal value </param>
        /// <param name="signalProvider"> Reference to the SignalProvider that sends this Signal </param>
        /// <param name="message"> Text message used to pass info about this Signal </param>
        /// <typeparam name="T"> Signal value type </typeparam>
        public static bool SendSignal<T>(string streamName, string streamCategory, T signalValue, SignalProvider signalProvider, string message = "") =>
            SendSignal(GetStream(streamName, streamCategory), signalValue, signalProvider, message);

        /// <summary> Send a MetaSignal on the stream with the given stream name and category, with a reference to the Object that sent it </summary>
        /// <param name="streamName"> Target stream name </param>
        /// <param name="streamCategory"> Target stream category </param>
        /// <param name="signalValue"> Signal value </param>
        /// <param name="signalSender"> Reference to the Object that sends this Signal </param>
        /// <param name="message"> Text message used to pass info about this Signal </param>
        /// <typeparam name="T"> Signal value type </typeparam>
        public static bool SendSignal<T>(string streamName, string streamCategory, T signalValue, Object signalSender, string message = "") =>
            SendSignal(GetStream(streamName, streamCategory), signalValue, signalSender, message);

        #endregion

        #region Send Signal - using a StreamName and default StreamCategory

        /// <summary> Send a Signal on the stream with the given stream name and default category </summary>
        /// <param name="streamName"> Target stream name </param>
        /// <param name="message"> Text message used to pass info about this Signal </param>
        public static bool SendSignal(string streamName, string message = "") =>
            SendSignal(GetStream(streamName), message);

        /// <summary> Send a Signal on the stream with the given stream name and default category, with a reference to the GameObject from where it is sent </summary>
        /// <param name="streamName"> Target stream name </param>
        /// <param name="signalSource"> Reference to the GameObject from where this Signal is sent </param>
        /// <param name="message"> Text message used to pass info about this Signal </param>
        public static bool SendSignal(string streamName, GameObject signalSource, string message = "") =>
            SendSignal(GetStream(streamName), signalSource, message);

        /// <summary> Send a Signal on the stream with the given stream name and default category, with a reference to the SignalProvider that sent it </summary>
        /// <param name="streamName"> Target stream name </param>
        /// <param name="signalProvider"> Reference to the SignalProvider that sends this Signal </param>
        /// <param name="message"> Text message used to pass info about this Signal </param>
        public static bool SendSignal(string streamName, SignalProvider signalProvider, string message = "") =>
            SendSignal(GetStream(streamName), signalProvider, message);

        /// <summary> Send a Signal on the stream with the given stream name and default category, with a reference to the Object that sent it </summary>
        /// <param name="streamName"> Target stream name </param>
        /// <param name="signalSender"> Reference to the Object that sends this Signal </param>
        /// <param name="message"> Text message used to pass info about this Signal </param>
        public static bool SendSignal(string streamName, Object signalSender, string message = "") =>
            SendSignal(GetStream(streamName), signalSender, message);

        #endregion

        #region Send MetaSignal - using a StreamName and default StreamCategory

        /// <summary> Send a MetaSignal on the stream with the given stream name and default category </summary>
        /// <param name="streamName"> Target stream name </param>
        /// <param name="signalValue"> Signal value </param>
        /// <param name="message"> Text message used to pass info about this Signal </param>
        public static bool SendSignal<T>(string streamName, T signalValue, string message = "") =>
            SendSignal(GetStream(streamName), signalValue, message);

        /// <summary> Send a MetaSignal on the stream with the given stream name and default category, with a reference to the GameObject from where it is sent </summary>
        /// <param name="streamName"> Target stream name </param>
        /// <param name="signalValue"> Signal value </param>
        /// <param name="signalSource"> Reference to the GameObject from where this Signal is sent </param>
        /// <param name="message"> Text message used to pass info about this Signal </param>
        /// <typeparam name="T"> Signal value type </typeparam>
        public static bool SendSignal<T>(string streamName, T signalValue, GameObject signalSource, string message = "") =>
            SendSignal(GetStream(streamName), signalValue, signalSource, message);

        /// <summary> Send a MetaSignal on the stream with the given stream name and default category, with a reference to the SignalProvider that sent it </summary>
        /// <param name="streamName"> Target stream name </param>
        /// <param name="signalValue"> Signal value </param>
        /// <param name="signalProvider"> Reference to the SignalProvider that sends this Signal </param>
        /// <param name="message"> Text message used to pass info about this Signal </param>
        /// <typeparam name="T"> Signal value type </typeparam>
        public static bool SendSignal<T>(string streamName, T signalValue, SignalProvider signalProvider, string message = "") =>
            SendSignal(GetStream(streamName), signalValue, signalProvider, message);

        /// <summary> Send a MetaSignal on the stream with the given stream name and default category, with a reference to the Object that sent it </summary>
        /// <param name="streamName"> Target stream name </param>
        /// <param name="signalValue"> Signal value </param>
        /// <param name="signalSender"> Reference to the Object that sends this Signal </param>
        /// <param name="message"> Text message used to pass info about this Signal </param>
        /// <typeparam name="T"> Signal value type </typeparam>
        public static bool SendSignal<T>(string streamName, T signalValue, Object signalSender, string message = "") =>
            SendSignal(GetStream(streamName), signalValue, signalSender, message);

        #endregion

        #region Send Signal - using a Guid streamKey

        /// <summary> Send a Signal on the stream with the given stream key (Guid) </summary>
        /// <param name="streamKey"> Target stream key (Guid) </param>
        /// <param name="message"> Text message used to pass info about this Signal </param>
        public static bool SendSignal(Guid streamKey, string message = "") =>
            SendSignal(FindStream(streamKey), message);

        /// <summary> Send a Signal on the stream with the given stream key (Guid), with a reference to the GameObject from where it was sent </summary>
        /// <param name="streamKey"> Target stream key (Guid) </param>
        /// <param name="signalSource"> Reference to the GameObject from where this Signal is sent from </param>
        /// <param name="message"> Text message used to pass info about this Signal </param>
        public static bool SendSignal(Guid streamKey, GameObject signalSource, string message = "") =>
            SendSignal(FindStream(streamKey), signalSource, message);

        /// <summary> Send a Signal on the stream with the given stream key (Guid), with a reference to the SignalProvider that sent it </summary>
        /// <param name="streamKey"> Target stream key (Guid) </param>
        /// <param name="signalProvider"> Reference to the SignalProvider that sends this Signal </param>
        /// <param name="message"> Text message used to pass info about this Signal </param>
        public static bool SendSignal(Guid streamKey, SignalProvider signalProvider, string message = "") =>
            SendSignal(FindStream(streamKey), signalProvider, message);

        /// <summary> Send a Signal on the stream with the given stream key (Guid), with a reference to the Object that sent it </summary>
        /// <param name="streamKey"> Target stream key (Guid) </param>
        /// <param name="signalSender"> Reference to the Object that sends this Signal </param>
        /// <param name="message"> Text message used to pass info about this Signal </param>
        public static bool SendSignal(Guid streamKey, Object signalSender, string message = "") =>
            SendSignal(FindStream(streamKey), signalSender, message);

        #endregion

        #region Send MetaSignal - using a Guid streamKey

        /// <summary> Send a MetaSignal on the stream with the given stream key (Guid) </summary>
        /// <param name="streamKey"> Target stream key (Guid) </param>
        /// <param name="signalValue"> Signal value </param>
        /// <param name="message"> Text message used to pass info about this Signal </param>
        /// <typeparam name="T"> Signal value type </typeparam>
        public static bool SendSignal<T>(Guid streamKey, T signalValue, string message = "") =>
            SendSignal(FindStream(streamKey), signalValue, message);

        /// <summary> Send a MetaSignal on the stream with the given stream key (Guid), with a reference to the GameObject from where it was sent </summary>
        /// <param name="streamKey"> Target stream key (Guid) </param>
        /// <param name="signalValue"> Signal value </param>
        /// <param name="signalSource"> Reference to the GameObject from where this Signal is sent from </param>
        /// <param name="message"> Text message used to pass info about this Signal </param>
        /// <typeparam name="T"> Signal value type </typeparam>
        public static bool SendSignal<T>(Guid streamKey, T signalValue, GameObject signalSource, string message = "") =>
            SendSignal(FindStream(streamKey), signalValue, signalSource, message);

        /// <summary> Send a MetaSignal on the stream with the given stream key (Guid), with a reference to the SignalProvider that sent it </summary>
        /// <param name="streamKey"> Target stream key (Guid) </param>
        /// <param name="signalValue"> Signal value </param>
        /// <param name="signalProvider"> Reference to the SignalProvider that sends this Signal </param>
        /// <param name="message"> Text message used to pass info about this Signal </param>
        /// <typeparam name="T"> Signal value type </typeparam>
        public static bool SendSignal<T>(Guid streamKey, T signalValue, SignalProvider signalProvider, string message = "") =>
            SendSignal(FindStream(streamKey), signalValue, signalProvider, message);

        /// <summary> Send a MetaSignal on the stream with the given stream key (Guid), with a reference to the Object that sent it </summary>
        /// <param name="streamKey"> Target stream key (Guid) </param>
        /// <param name="signalValue"> Signal value </param>
        /// <param name="signalSender"> Reference to the Object that sends this Signal </param>
        /// <param name="message"> Text message used to pass info about this Signal </param>
        /// <typeparam name="T"> Signal value type </typeparam>
        public static bool SendSignal<T>(Guid streamKey, T signalValue, Object signalSender, string message = "") =>
            SendSignal(FindStream(streamKey), signalValue, signalSender, message);

        #endregion

        #region Send Signal - using a SignalStream reference

        /// <summary> Send a Signal on the given stream </summary>
        /// <param name="stream"> Target signal stream </param>
        /// <param name="message"> Text message used to pass info about this Signal </param>
        public static bool SendSignal(SignalStream stream, string message = "") =>
            stream != null && stream.SendSignal(message);

        /// <summary> Send a Signal on the given stream, with a reference to the GameObject from where it was sent </summary>
        /// <param name="stream"> Target signal stream </param>
        /// <param name="signalSource"> Reference to the GameObject from where this Signal is sent from </param>
        /// <param name="message"> Text message used to pass info about this Signal </param>
        public static bool SendSignal(SignalStream stream, GameObject signalSource, string message = "") =>
            stream != null && stream.SendSignal(signalSource, message);

        /// <summary> Send a Signal on the given stream, with a reference to the SignalProvider that sent it </summary>
        /// <param name="stream"> Target signal stream </param>
        /// <param name="signalProvider"> Reference to the SignalProvider that sends this Signal </param>
        /// <param name="message"> Text message used to pass info about this Signal </param>
        public static bool SendSignal(SignalStream stream, SignalProvider signalProvider, string message = "") =>
            stream != null && stream.SendSignal(signalProvider, message);

        /// <summary> Send a Signal on the given stream, with a reference to the Object that sent it </summary>
        /// <param name="stream"> Target signal stream </param>
        /// <param name="signalSender"> Reference to the Object that sends this Signal </param>
        /// <param name="message"> Text message used to pass info about this Signal </param>
        public static bool SendSignal(SignalStream stream, Object signalSender, string message = "") =>
            stream != null && stream.SendSignal(signalSender, message);

        #endregion

        #region Send MetaSignal - using a SignalStream reference

        /// <summary> Send a MetaSignal on the given stream </summary>
        /// <param name="stream"> Target signal stream </param>
        /// <param name="signalValue"> Signal value </param>
        /// <param name="message"> Text message used to pass info about this Signal </param>
        /// <typeparam name="T"> Signal value type </typeparam>
        public static bool SendSignal<T>(SignalStream stream, T signalValue, string message = "") =>
            stream != null && stream.SendSignal(signalValue, message);

        /// <summary> Send a MetaSignal on the given stream, with a reference to the GameObject from where it was sent </summary>
        /// <param name="stream"> Target signal stream </param>
        /// <param name="signalValue"> Signal value </param>
        /// <param name="signalSource"> Reference to the GameObject from where this Signal is sent from </param>
        /// <param name="message"> Text message used to pass info about this Signal </param>
        /// <typeparam name="T"> Signal value type </typeparam>
        public static bool SendSignal<T>(SignalStream stream, T signalValue, GameObject signalSource, string message = "") =>
            stream != null && stream.SendSignal(signalValue, signalSource, message);

        /// <summary> Send a MetaSignal on the given stream, with a reference to the SignalProvider that sent it </summary>
        /// <param name="stream"> Target signal stream </param>
        /// <param name="signalValue"> Signal value </param>
        /// <param name="signalProvider"> Reference to the SignalProvider that sends this Signal </param>
        /// <param name="message"> Text message used to pass info about this Signal </param>
        /// <typeparam name="T"> Signal value type </typeparam>
        public static bool SendSignal<T>(SignalStream stream, T signalValue, SignalProvider signalProvider, string message = "") =>
            stream != null && stream.SendSignal(signalValue, signalProvider, message);

        /// <summary> Send a MetaSignal on the given stream, with a reference to the Object that sent it </summary>
        /// <param name="stream"> Target signal stream </param>
        /// <param name="signalValue"> Signal value </param>
        /// <param name="signalSender"> Reference to the Object that sends this Signal </param>
        /// <param name="message"> Text message used to pass info about this Signal </param>
        /// <typeparam name="T"> Signal value type </typeparam>
        public static bool SendSignal<T>(SignalStream stream, T signalValue, Object signalSender, string message = "") =>
            stream != null && stream.SendSignal(signalValue, signalSender, message);

        #endregion

        #endregion
    }
}
