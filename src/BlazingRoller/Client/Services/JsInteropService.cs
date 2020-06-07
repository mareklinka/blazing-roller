using System.Text.Json;
using System.Threading.Tasks;
using BlazingRoller.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazingRoller.Client.Services
{
    public class JsInteropService
    {
        private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions { IgnoreNullValues = true };

        private readonly IJSRuntime _jsRuntime;

        public JsInteropService(IJSRuntime jsRuntime) => _jsRuntime = jsRuntime;

        public ValueTask InitializeWebGl(string elementSelector) =>
            _jsRuntime.InvokeVoidAsync("BlazingRollerLib.init", elementSelector);

        public ValueTask ScrollElementToEnd(ElementReference element) =>
            _jsRuntime.InvokeVoidAsync("ElementInterop.scrollToEnd", element);

        public ValueTask RepositionDice(DieFinalConfigurationWrapper config)
        {
            var json = JsonSerializer.Serialize(config, _jsonOptions);
            return _jsRuntime.InvokeVoidAsync("engine.repositionDice", json);
        }

        public ValueTask SendRollToUnity(DiceThrowConfiguration config)
        {
            var json = JsonSerializer.Serialize(config, _jsonOptions);
            return _jsRuntime.InvokeVoidAsync("engine.rollDice", json);
        }
    }
}
