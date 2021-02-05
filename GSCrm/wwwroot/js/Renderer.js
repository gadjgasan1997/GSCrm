class Renderer {
    static Render() {
        let appData = JSON.parse(localStorage.getItem("GSCrmAppData"));
        let viewInfo = appData["viewInfo"];
        if (!Utils.IsNullOrEmpty(viewInfo) && !Utils.IsNullOrEmpty(viewInfo["renderName"])) {
            let renderClass = Renderer.CreateRenderInstanse(viewInfo["renderName"]);
            if (!Utils.IsNullOrEmpty(renderClass)) {
                renderClass.Render();
            }
        }
    }

    /**
     * Метод создает рендер по его названию
     * @param {*} renderName 
     */
    static CreateRenderInstanse(renderName) {
        return new Map([
            ['ProductCategoriesRender', new ProductCategoriesRender()]
        ]).get(renderName)
    }
}