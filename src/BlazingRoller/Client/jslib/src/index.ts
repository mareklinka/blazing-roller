export class DiceEngine {
    private unityInstance: any;
    initialize(): void {
        this.unityInstance = (window["UnityLoader"] as any).instantiate("unityContainer", "Build/wwwroot.json", {onProgress: window["UnityProgress"]});
    }


    rollDice(seed: number): void {
        if (!this.unityInstance) {
            return;
        }

        this.unityInstance.SendMessage("DiceManager", "NewThrow", seed);
    }
}

export function init(): void {
    const engine = new DiceEngine();
    window["engine"] = engine;
    engine.initialize();
}