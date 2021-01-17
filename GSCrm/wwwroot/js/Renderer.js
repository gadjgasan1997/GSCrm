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
     * ����� ������� ������ �� ��� ��������
     * @param {*} renderName 
     */
    static CreateRenderInstanse(renderName) {
        return new Map([
            ['ProductCategoriesRender', new ProductCategoriesRender()],
            ['Goo', new ProductCategoriesRender()]
        ]).get(renderName)
    }
}